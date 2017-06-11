using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ForumAPI.Data;
using Newtonsoft.Json;
using ForumAPI.Models;
using ForumAPI.Utilities;
using ForumAPI.Services;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ForumAPI.Controllers
{
    /// <summary>
    /// Controller that returns information about threads, which includes the
    /// posts when requesting a single thread.
    /// </summary>
    [Route("api/[controller]")]
    public class ThreadsController : Controller
    {
        private IForumContext database;
        private ILoginSessionService logins;

        public ThreadsController(ILoginSessionService logins, IForumContext database)
        {
            this.logins = logins;
            this.database = database;
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            Thread thread = database.GetThread(id);
            if (thread == null)
            {
                return NotFound(Errors.NoSuchElement);
            }
            return new ObjectResult(thread);
        }

        [HttpGet("{id}/posts")]
        public IActionResult GetPosts(int id)
        {
            Thread thread = database.GetThread(id);

            if (thread == null)
            {
                return NotFound(Errors.NoSuchElement);
            }

            return new ObjectResult(thread.Posts);
        }

        [HttpPost]
        public IActionResult Create([FromBody]string session, [FromBody]Thread thread)
        {
            if (thread == null)
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
                if (user.Status >= UserStatus.Active)
                {
                    thread.AuthorID = user.ID;
                    database.AddThread(thread);
                    database.SaveChanges();
                    logins.UpdateLoginAction(session);
                    return new ObjectResult(thread);
                }
                else
                {
                    return new ForbidResult(Errors.PermissionDenied);
                }
            }
        }

        [HttpPost("{id}/posts")]
        public IActionResult AddPost(int id, [FromBody]string session, [FromBody]Post post)
        {
            if (post == null)
            {
                return BadRequest(Errors.MissingFields);
            }

            if (database.GetThread(id) == null)
            {
                return NotFound(Errors.NoSuchElement);
            }

            post.OwnerID = id;

            string error = "";
            User user = logins.GetUser(session, out error);
            if (user == null)
            {
                return BadRequest(error);
            }
            else
            {
                if (user.Status >= UserStatus.Active)
                {
                    post.AuthorID = user.ID;
                    database.AddPost(post);
                    database.SaveChanges();
                    logins.UpdateLoginAction(session);
                    return new ObjectResult(post);
                }
                else
                {
                    return new ForbidResult(Errors.PermissionDenied);
                }
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id, [FromBody]string session)
        {
            Thread thread = database.GetThread(id);
            if (thread == null)
            {
                return NotFound(Errors.NoSuchElement);
            }
            string error = "";
            User user = logins.GetUser(session, out error);
            if (user == null)
            {
                return BadRequest(error);
            }
            else
            {
                if (thread.Author == user || user.Status >= UserStatus.Moderator)
                {
                    database.RemoveThread(thread);
                    database.SaveChanges();
                    logins.UpdateLoginAction(session);
                    return new ObjectResult(thread);
                }
                else
                {
                    return new ForbidResult(Errors.PermissionDenied);
                }
            }

        }
    }
}
