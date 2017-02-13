using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumAPI.Models
{
    /// <summary>
    /// The model that keeps track of logins so that a user can login once then
    /// use the token to perform actions.
    /// </summary>
    public class LoginSession
    {
        public string Token { get; set; }
        public int UserID { get; set; }
        public User User { get; set; }
        public DateTime LastAction { get; set; }
    }
}
