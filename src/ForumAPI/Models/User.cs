using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [Key]
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }

        public DateTime Created { get; set; }
        public UserStatus Status { get; set; }
        
        // Passwords must be encrypted for the purpose of security, even if
        // this will not see commercial use.
        [JsonIgnore]
        public string SHA256Password { get; set; }
        
        // A random string attached to the password before encryption.
        [JsonIgnore]
        public string Salt { get; set; }

        [JsonIgnore]
        public int PasswordProtocolVersion { get; set; }

        [Required]
        public int PortraitID { get; set; }
        [ForeignKey("PortraitID")]
        public Picture Portrait { get; set; }

        [Required]
        public bool HasSignature { get; set; }
        public string Signature { get; set; }

        public ICollection<Thread> Threads { get; set; }
        public ICollection<Post> Posts { get; set; }
    }
}
