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
        ILoginSessionService logins;

        public BoardController(ILoginSessionService logins, IForumContext database)
        {
            this.logins = logins;
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

            if (topic.SubTopics != null)
            {
                return new ObjectResult(topic.SubTopics.Select(t => t.Child));
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
            // Return all threads by which has the most recent (latest) post
            if (topic.Threads != null)
            {
                return new ObjectResult(topic.Threads.OrderByDescending(
                           t => t.Posts.Count > 1 ? t.Posts.Last().Created : t.Created));
            }
            // We need to return an empty list to the client to maintain consistency, if no
            // threads are on the board.
            else return new ObjectResult(new LinkedList<Thread>());
        }

        [HttpPost]
        public IActionResult NewTopic([FromQuery]string session, [FromBody]Topic topic)
        {
            if (topic == null)
            {
                return BadRequest(Errors.MissingFields);
            }

            string error = "";
            User user = logins.GetUser(session, out error);
            
            if (user == null)
            {
                return BadRequest(error);
            }
            else
            {
                if (user.Status == UserStatus.Administrator)
                {
                    database.AddTopic(topic);
                    database.SaveChanges();
                    logins.UpdateLoginAction(session);
                    return new ObjectResult(topic);
                }
                else
                {
                    return new ForbidResult(Errors.PermissionDenied);
                }
            }
            
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id, [FromQuery]string session)
        {
            Topic topic = database.GetTopic(id);

            string error = "";
            User user = logins.GetUser(session, out error);

            if (user == null)
            {
                return BadRequest(error);
            }
            else
            {
                if (user.Status == UserStatus.Administrator)
                {
                    database.RemoveTopic(topic);
                    database.SaveChanges();
                    logins.UpdateLoginAction(session);
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
