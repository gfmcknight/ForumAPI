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
    /// A controller which verifies logins and creates sessions whereby a user
    /// can make posts with a single token.
    /// </summary>
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        LoginSessionService logins;
        ForumContext database;

        public LoginController(LoginSessionService logins, ForumContext database)
        {
            this.logins = logins;
            this.database = database;
        }

        /// <summary>
        /// Logs a user in with a username and a password.
        /// </summary>
        /// <param name="username">The user's username or email.</param>
        /// <param name="password">The user's password.</param>
        /// <returns>The session token that gets created.</returns>
        [HttpPost]
        public string Post([FromBody]string username, [FromBody]string password)
        {
            User user = database.GetUser(username);
            if (user == null)
            {
                // Don't return a token if they use an invalid login and handle
                // issue on the client side.
                return "";
            }
            if (DataHandler.IsCorrectPassword(password, user))
            {
                return logins.AddLogin(user);
            }
            else
            {
                return "";
            }
        }

        [HttpDelete("{session}")]
        public void Delete(string session)
        {
            logins.
        }
    }
}
