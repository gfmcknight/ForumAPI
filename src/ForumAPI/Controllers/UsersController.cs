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

        LoginSessionService logins;
        ForumContext database;
        
        public UsersController(LoginSessionService logins, ForumContext database)
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

            foreach (User other in database.Users)
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

        // PATCH api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete([FromBody]string session, int id)
        {

        }
    }
}
