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

        public ThreadsController(IForumContext database)
        {
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

            return new ObjectResult(database.GetPosts(thread));
        }

        [HttpPost]
        public IActionResult Create([FromHeader]string session, [FromBody]Thread thread)
        {
            if (thread == null)
            {
                return BadRequest(Errors.MissingFields);
            }

            int userID;
            UserStatus status;
            if (!DataHandler.DecodeJWT(session, out userID, out status))
            {
                return Unauthorized();
            }
            else
            {
                if (status >= UserStatus.Active)
                {
                    thread.AuthorID = userID;
                    database.AddThread(thread);
                    database.SaveChanges();
                    return new ObjectResult(thread);
                }
                else
                {
                    return new ForbidResult(Errors.PermissionDenied);
                }
            }
        }

        [HttpPost("{id}/posts")]
        public IActionResult AddPost(int id, [FromHeader]string session, [FromBody]Post post)
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

            int userID;
            UserStatus status;
            if (!DataHandler.DecodeJWT(session, out userID, out status))
            {
                return Unauthorized();
            }
            else
            {
                if (status >= UserStatus.Active)
                {
                    post.AuthorID = userID;
                    database.AddPost(post);
                    database.SaveChanges();
                    return new ObjectResult(post);
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
            Thread thread = database.GetThread(id);
            if (thread == null)
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
                if (thread.AuthorID == userID || 
                    status >= UserStatus.Moderator)
                {
                    database.RemoveThread(thread);
                    database.SaveChanges();
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
