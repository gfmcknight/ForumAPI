using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumAPI.Services
{
    /// <summary>
    /// A username/password pair to recieve when the user logs in.
    /// </summary>
    public class LoginPair
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
