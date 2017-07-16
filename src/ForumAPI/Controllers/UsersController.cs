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

        IForumContext database;

        public UsersController(IForumContext database)
        {
            this.database = database;
        }

        /// <summary>
        /// Finds the user with the given id.
        /// </summary>
        /// <param name="id">The id of the user to find.</param>
        /// <returns>The user with the given id, if found,
        /// otherwise a response 404.</returns>
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

        /// <summary>
        /// Returns the user given an authentication token.
        /// </summary>
        /// <param name="session">Authentication token from the login service.</param>
        /// <returns>The user owning the token, if found,
        /// otherwise a response 404.</returns>
        [HttpGet("self")]
        public IActionResult GetSelf([FromHeader]string session)
        {
            int userID;
            UserStatus status;

            if (!DataHandler.DecodeJWT(session, out userID, out status))
            {
                return NotFound(Errors.SessionNotFound);
            }

            User user = database.GetUser(userID);

            if (user == null)
            {
                return NotFound(Errors.NoSuchElement);
            }

            user.Email = null;

            return new ObjectResult(user);
        }

        /// <summary>
        /// Posts a new user.
        /// </summary>
        /// <param name="user">The user to create.</param>
        /// <returns>
        /// Response 201 if the user is correctly created.
        /// Response 400 if the user object doesn't contain the necessary fields.
        /// </returns>
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

            if (database.GetAllUsers().FirstOrDefault(u => u.Name == user.Username ||
                                                      u.Email == user.Email) != null)
            {
                return BadRequest(Errors.AlreadyExists);
            }

            if (!DataHandler.VerifyPassword(user.Password))
            {
                return BadRequest(Errors.WeakPassword);
            }

            User newProfile = database.AddUser(user);
            database.SaveChanges();
            user.Password = "";
            return new CreatedResult("users/" + newProfile.ID, user);
        }

        /// <summary>
        /// Modifies a user with the given fields.
        /// </summary>
        /// <param name="id">The id of the user to modify.</param>
        /// <param name="user">The set of changes to make to the user.</param>
        /// <param name="session">Authentication token from the login service.</param>
        /// <returns>
        /// Response 200 if sucessful in modifying user.
        /// Response 400 if the user object doesn't contain the necessary fields.
        /// Response 401 if the session is empty, expired, or invalid.
        /// Response 403 if the user is banned or deactivated, or is attempting to
        /// modify a user that is not themself, unless they are a moderator.
        /// Response 404 if the user doesn't exist with the given id.
        /// </returns>
        [HttpPut("{id}")]
        public IActionResult Modify(int id, [FromBody]UserSubmission user, [FromHeader]string session)
        {
            if (user == null)
            {
                return BadRequest(Errors.MissingFields);
            }

            User userInDatabase = database.GetUser(id);

            if (userInDatabase == null)
            {
                return NotFound(Errors.NoSuchElement);
            }

            int requesterID;
            UserStatus requesterStatus;
            if (!DataHandler.DecodeJWT(session, out requesterID, out requesterStatus))
            {
                return Unauthorized();
            }

            if (requesterStatus < UserStatus.Active)
            {
                return new ForbidResult();
            }

            if (userInDatabase.ID != requesterID &&
                user.Password != null && user.Password != "")
            {
                return new ForbidResult();
            }
            else
            {
                // A user can only make a change to themselves or if they are a moderator
                if (requesterID == userInDatabase.ID ||
                    requesterStatus >= UserStatus.Moderator)
                {
                    database.UpdateUser(userInDatabase.ID, user, requesterStatus);
                    database.SaveChanges();
                    return new ObjectResult(database.GetUser(userInDatabase.ID));
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
        /// <param name="id">The id of the user to delete.</param>
        /// <param name="session">Authentication token from the login service.</param>
        /// <returns>
        /// Response 200 if the user is correctly deleted.
        /// Response 401 if the session is empty, expired, or invalid.
        /// Response 403 if the user is not an administrator.
        /// Response 404 if the user cannot be found.
        /// </returns>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id, [FromHeader]string session)
        {
            User user = database.GetUser(id);
            if (user == null)
            {
                return NotFound(Errors.NoSuchElement);
            }

            int requesterID;
            UserStatus requesterStatus;

            if (!DataHandler.DecodeJWT(session, out requesterID, out requesterStatus))
            {
                return Unauthorized();
            }
            else
            {
                if (requesterStatus == UserStatus.Administrator)
                {
                    database.RemoveUser(user);
                    database.SaveChanges();
                    return new ObjectResult(user);
                }
                else
                {
                    return new ForbidResult();
                }
            }
        }
    }
}
