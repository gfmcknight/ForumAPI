using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ForumAPI.Data;
using ForumAPI.Models;
using ForumAPI.Utilities;
using ForumAPI.Services;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

///
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

        [HttpGet]
        public IActionResult GetRoot()
        {
            return new ObjectResult(database.GetTopic(1));
        }

        [HttpGet("{id}")]
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
                    return new ObjectResult(topic);
                }
                else
                {
                    return new ForbidResult(Errors.PermissionDenied);
                }
            }
            
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id, [FromHeader]string session)
        {
            Topic topic = database.GetTopic(id);

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
                    return new ForbidResult(Errors.PermissionDenied);
                }
            }
        }
    }
}
