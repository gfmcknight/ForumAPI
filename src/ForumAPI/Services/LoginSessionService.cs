using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForumAPI.Models;

namespace ForumAPI.Services
{
    /// <summary>
    /// A service that keeps track of users that are logged in to the
    /// API.
    /// </summary>
    public class LoginSessionService
    {
        private Dictionary<String, User> userLogins;
        public LoginSessionService()
        {

        }
    }
}
