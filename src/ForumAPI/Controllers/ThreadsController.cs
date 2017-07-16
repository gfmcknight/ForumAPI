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

        /// <summary>
        /// Finds the thread with the given id.
        /// </summary>
        /// <param name="id">The id of the thread to find.</param>
        /// <returns>The thread, if found, otherwise a response 404.</returns>
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

        /// <summary>
        /// Returns a list of all posts in a given thread.
        /// </summary>
        /// <param name="id">The id of the thread.</param>
        /// <returns>The list of posts if the thread exists,
        /// otherwise a response 404.</returns>
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

        /// <summary>
        /// Posts a new thread.
        /// </summary>
        /// <param name="session">Authentication token from the login service.</param>
        /// <param name="thread">The thread to post.</param>
        /// <returns>
        /// Response 201 if the thread is correctly created, containing the path
        /// to the thread and the newly created thread object.
        /// Response 400 if the thread object doesn't contain necessary fields.
        /// Response 401 if the session is empty, expired, or invalid.
        /// Response 403 if the user is deactivated or banned.
        /// Response 404 if the thread's owner topic doesn't exist
        /// </returns>
        [HttpPost]
        public IActionResult Create([FromHeader]string session, [FromBody]Thread thread)
        {
            if (thread == null)
            {
                return BadRequest(Errors.MissingFields);
            }

            Topic topic = database.GetTopic(thread.OwnerID);
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
                if (status >= UserStatus.Active)
                {
                    thread.AuthorID = userID;
                    database.AddThread(thread);
                    database.SaveChanges();
                    return new CreatedResult("threads/" + thread.ID, thread);
                }
                else
                {
                    return new ForbidResult();
                }
            }
        }

        /// <summary>
        /// Attaches a new post to a thread.
        /// </summary>
        /// <param name="id">The id of the thread to attach to.</param>
        /// <param name="session">Authentication token from the login service.</param>
        /// <param name="post">The post to post.</param>
        /// <returns>
        /// Response 201 if the post is correctly created, containing the path
        /// to the overall thread and the newly created post object.
        /// Response 400 if the post object doesn't contain necessary fields.
        /// Response 401 if the session is empty, expired, or invalid.
        /// Response 403 if the user is deactivated or banned.
        /// Response 404 if the thread cannot be found.
        /// </returns>
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
                    return new CreatedResult("threads/" + id, post);
                }
                else
                {
                    return new ForbidResult();
                }
            }
        }

        /// <summary>
        /// Deletes the given thread, and all posts within.
        /// </summary>
        /// <param name="id">The id of the thread to delete.</param>
        /// <param name="session">Authentication token from the login service.</param>
        /// <returns>
        /// Response 200 if the thread is correctly deleted.
        /// Response 401 if the session is empty, expired, or invalid.
        /// Response 403 if the user is not the original poster or a moderator.
        /// Response 404 if the thread cannot be found.
        /// </returns>
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
                    return new ForbidResult();
                }
            }

        }
    }
}
