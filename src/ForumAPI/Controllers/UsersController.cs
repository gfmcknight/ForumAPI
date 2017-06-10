using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ForumAPI.Services;
using ForumAPI.Data;
using ForumAPI.Models;
using ForumAPI.Utilities;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ForumAPI.Controllers
{
    /// <summary>
    /// Controller that returns information about users.
    /// </summary>
    [Route("api/[controller]")]
    public class UsersController : Controller
    {

        ILoginSessionService logins;
        IForumContext database;

        public UsersController(ILoginSessionService logins, IForumContext database)
        {
            this.logins = logins;
            this.database = database;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            User user = database.GetUser(id);

            if (user == null)
            {
                return NotFound(Errors.NoSuchElement);
            }

            user.Email = null;

            return new ObjectResult(user);
        }

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody]User user, [FromBody]string password)
        {
            if (user == null)
            {
                return BadRequest(Errors.MissingFields);
            }

            if (!DataHandler.IsValidEmail(user.Email))
            {
                return BadRequest(Errors.InvalidEmail);
            }

            foreach (User other in database.GetAllUsers())
            {
                if (user.Name == other.Name || user.Email == other.Email)
                {
                    return BadRequest(Errors.AlreadyExists);
                }
            }

            if (!DataHandler.VerifyPassword(password))
            {
                return BadRequest(Errors.WeakPassword);
            }

            database.AddUser(user, password);
            database.SaveChanges();
            return new ObjectResult(user);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public IActionResult Put([FromBody]User user, [FromBody]string password, [FromBody]string session)
        {
            if (user == null)
            {
                return BadRequest(Errors.MissingFields);
            }

            if (database.GetUser(user.ID) == null)
            {
                return NotFound(Errors.NoSuchElement);
            }

            string error = "";
            User requester = logins.GetUser(session, out error);

            if (requester.Status < UserStatus.Active)
            {
                return new ForbidResult(Errors.PermissionDenied);
            }

            if (session == null)
            {
                return BadRequest(error);
            }
            else
            {
                // If the user is requesting a password change, they must be user they
                // want to apply it to.
                if (user.ID != requester.ID && (password != null && password != ""))
                {
                    return new ForbidResult(Errors.PermissionDenied);
                }
                else
                {
                    // A user can only make a change to themselves or if they are a moderator
                    if (requester.ID == user.ID || requester.Status >= UserStatus.Moderator)
                    {
                        database.UpdateUser(user, password, requester.Status);
                        database.SaveChanges();
                        return new ObjectResult(database.GetUser(user.ID));
                    }
                    else
                    {
                        return new ForbidResult(Errors.PermissionDenied);
                    }
                }
            }

        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id, [FromBody]string session)
        {
            User user = database.GetUser(id);
            if (user == null)
            {
                return NotFound(Errors.NoSuchElement);
            }

            string error = "";
            User requester = logins.GetUser(session, out error);
            if (requester == null)
            {
                return BadRequest(error);
            }
            else
            {
                if (requester.Status == UserStatus.Administrator)
                {
                    database.RemoveUser(user);
                    database.SaveChanges();
                    return new ObjectResult(user);
                }
                else
                {
                    return new ForbidResult(Errors.PermissionDenied);
                }
            }
        }
    }
}
