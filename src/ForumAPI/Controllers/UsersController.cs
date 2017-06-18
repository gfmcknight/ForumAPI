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
        public IActionResult NewUser([FromBody]UserSubmission user)
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
                if (user.Username == other.Name || user.Email == other.Email)
                {
                    return BadRequest(Errors.AlreadyExists);
                }
            }

            if (!DataHandler.VerifyPassword(user.Password))
            {
                return BadRequest(Errors.WeakPassword);
            }

            database.AddUser(user);
            database.SaveChanges();
            user.Password = "";
            return new ObjectResult(user);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public IActionResult Modify([FromBody]UserSubmission user, [FromBody]string session)
        {
            if (user == null)
            {
                return BadRequest(Errors.MissingFields);
            }

            User userInDatabase = database.GetUser(user.Username);

            if (userInDatabase == null)
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
                if (userInDatabase.ID != requester.ID && 
                    user.Password != null && user.Password != "")
                {
                    return new ForbidResult(Errors.PermissionDenied);
                }
                else
                {
                    // A user can only make a change to themselves or if they are a moderator
                    if (requester.ID == userInDatabase.ID || 
                        requester.Status >= UserStatus.Moderator)
                    {
                        database.UpdateUser(userInDatabase.ID, user, requester.Status);
                        database.SaveChanges();
                        return new ObjectResult(database.GetUser(userInDatabase.ID));
                    }
                    else
                    {
                        return new ForbidResult(Errors.PermissionDenied);
                    }
                }
            }

        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id, [FromQuery]string session)
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
