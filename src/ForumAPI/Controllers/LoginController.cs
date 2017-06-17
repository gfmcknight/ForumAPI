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
        ILoginSessionService logins;
        IForumContext database;

        public LoginController(ILoginSessionService logins, IForumContext database)
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
        public string Login([FromBody]LoginPair login)
        {
            if (login.Username == null || login.Password == null)
            {

                return "";
            }
            User user = database.GetUser(login.Username);
            if (user == null)
            {
                // Don't return a token if they use an invalid login and handle
                // issue on the client side.
                return "";
            }
            if (DataHandler.IsCorrectPassword(login.Password, user))
            {
                return logins.AddLogin(user);
            }
            else
            {
                return "";
            }
        }

        [HttpDelete("{session}")]
        public void Logout(string session)
        {
            string error = "";
            User user = logins.GetUser(session, out error);
            if (user != null)
            {
                logins.Logout(session);
            }
        }
    }
}
