using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ForumAPI.Data;
using ForumAPI.Models;
using ForumAPI.Utilities;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ForumAPI.Controllers
{
    /// <summary>
    /// A controller which can be used for navigation in the ForumAPI.
    /// </summary>
    [Route("api/[controller]")]
    public class BoardController : Controller
    {
        IForumContext database;

        public BoardController(IForumContext database)
        {
            this.database = database;
        }
        
        /// <summary>
        /// Returns the root topic of the board.
        /// </summary>
        /// <returns>The root topic of the board.</returns>
        [HttpGet]
        public IActionResult GetRoot()
        {
            return new ObjectResult(database.GetTopic(1));
        }

        /// <summary>
        /// Finds the topic with the given id.
        /// </summary>
        /// <param name="id">The id of the topic to find.</param>
        /// <returns>The topic, if found, otherwise a response 404.</returns>
        [HttpGet("{id}")]
        public IActionResult GetTopic(int id)
        {
            Topic topic = database.GetTopic(id);
            if (topic == null)
            {
                return NotFound();
            }
            else
            {
                return new ObjectResult(topic);
            }
        }

        /// <summary>
        /// Returns a list of all subtopics in a topic.
        /// </summary>
        /// <param name="id">The id of the topic to find.</param>
        /// <returns>The list of all subtopics if the topic exists,
        /// otherwise a response 404.</returns>
        [HttpGet("{id}/topics")]
        public IActionResult GetSubtopics(int id)
        {
            Topic topic = database.GetTopic(id);
            if (topic == null)
            {
                return NotFound(Errors.NoSuchElement);
            }

            ICollection<Topic> subTopics = database.GetSubtopics(topic)
                                                   .Select(t => t.Child).ToList();

            if (subTopics != null)
            {
                return new ObjectResult(subTopics);
            }
            // Return an empty list to the client in the case that there are no
            // subtopics.
            else return new ObjectResult(new LinkedList<Thread>());
        }

        /// <summary>
        /// Returns a list of all threads in a topic.
        /// </summary>
        /// <param name="id">The id of the topic to find.</param>
        /// <returns>The list of all threads if the topic exists,
        /// otherwise a response 404.</returns>
        [HttpGet("{id}/threads")]
        public IActionResult GetThreads(int id)
        {
            Topic topic = database.GetTopic(id);

            if (topic == null)
            {
                return NotFound(Errors.NoSuchElement);
            }

            ICollection<Thread> threads = database.GetThreads(topic);

            // Return all threads by which has the most recent (latest) post
            if (threads != null)
            {
                return new ObjectResult(threads.OrderByDescending(
                           t => t.Posts != null && t.Posts.Count > 1 ? 
                           t.Posts.Last().Created : t.Created));
            }
            // We need to return an empty list to the client to maintain consistency, if no
            // threads are on the board.
            else return new ObjectResult(new LinkedList<Thread>());
        }

        /// <summary>
        /// Posts a new topic.
        /// </summary>
        /// <param name="id">The topic of the parent topic to attach to.</param>
        /// <param name="session">Authentication token from the login service.</param>
        /// <param name="topic">The topic to post.</param>
        /// <returns>
        /// Response 201 if the board is correctly created.
        /// Response 400 if the board object doesn't contain the necessary fields.
        /// Response 401 if the session is empty, expired, or invalid.
        /// Response 403 if the user is not an administrator.
        /// Response 404 if the parent thread does not exist.
        /// </returns>
        [HttpPost("{id}")]
        public IActionResult NewTopic(int id, [FromHeader]string session, [FromBody]Topic topic)
        {
            if (topic == null)
            {
                return BadRequest(Errors.MissingFields);
            }

            Topic parent = database.GetTopic(id);
            if (parent == null)
            {
                return NotFound(Errors.NoSuchElement);
            }

            int userID;
            UserStatus status;

            if (!DataHandler.DecodeJWT(session, out userID, out status))
            {
                return Unauthorized();
            }
            else
            {
                if (status == UserStatus.Administrator)
                {
                    database.AddTopic(topic);
                    database.AddTopicRelation(new TopicRelation
                        {
                            Parent = parent,
                            Child = topic
                        });

                    database.SaveChanges();
                    return new CreatedResult("board/" + topic.ID, topic);
                }
                else
                {
                    return new ForbidResult();
                }
            }
            
        }

        /// <summary>
        /// Deletes the given topic, and all threads and posts within.
        /// </summary>
        /// <param name="id">The id of the topic to delete.</param>
        /// <param name="session">Authentication token from the login service.</param>
        /// <returns>
        /// Response 200 if the topic is correctly deleted.
        /// Response 401 if the session is empty, expired, or invalid.
        /// Response 403 if the user is not an administrator.
        /// Response 404 if the thread cannot be found.
        /// </returns>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id, [FromHeader]string session)
        {
            Topic topic = database.GetTopic(id);
            if (topic == null)
            {
                return NotFound();
            }

            int userID;
            UserStatus status;

            if (!DataHandler.DecodeJWT(session, out userID, out status))
            {
                return Unauthorized();
            }
            else
            {
                if (status == UserStatus.Administrator)
                {
                    database.RemoveTopic(topic);
                    database.SaveChanges();
                    return new ObjectResult(topic);
                }
                else
                {
                    return new ForbidResult();
                }
            }
        }
    }
}
