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

        // PUT api/values/5
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
                return new ForbidResult(Errors.PermissionDenied);
            }

            if (userInDatabase.ID != requesterID &&
                user.Password != null && user.Password != "")
            {
                return new ForbidResult(Errors.PermissionDenied);
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
                    return new ForbidResult(Errors.PermissionDenied);
                }
            }
        }


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
                    return new ForbidResult(Errors.PermissionDenied);
                }
            }
        }
    }
}
