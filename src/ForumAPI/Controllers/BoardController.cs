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
        ForumContext database;
        LoginSessionService logins;

        // GET: api/topics
        [HttpGet]
        public IActionResult GetRoot()
        {
            return new ObjectResult(database.GetTopic(0));
        }

        // GET api/topics/5/threads
        [HttpGet("{id}/threads")]
        public IActionResult Get(int id)
        {
            Topic topic = database.GetTopic(id);

            if (topic == null)
            {
                return NotFound(Errors.NoSuchElement);
            }

            return new ObjectResult(topic.Threads);
        }

        // POST api/topics
        [HttpPost]
        public IActionResult Post([FromBody]string session, [FromBody]Topic topic)
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

        // DELETE api/topics/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id, [FromBody]string session)
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
