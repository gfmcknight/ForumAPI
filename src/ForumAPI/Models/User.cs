using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumAPI.Models
{
    public enum UserStatus
    {
        Administrator, Moderator, Active, Deactivated, Banned
    }

    /// <summary>
    /// The model for a user. Contains both login information and posting
    /// information.
    /// </summary>
    public class User
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime Created { get; set; }
        public UserStatus Status { get; set; }
        
        // Passwords must be encrypted for the purpose of security, even if
        // this will not see commercial use.
        public string SHA256Password { get; set; }
        // A random string attached to the password before encryption.
        public string Salt { get; set; }

        public Picture Portrait { get; set; }
        public int PortraitID { get; set; }

        public bool HasSignature { get; set; }
        public string Signature { get; set; }

        public ICollection<Thread> Threads { get; set; }
        public ICollection<Post> Posts { get; set; }
    }
}
