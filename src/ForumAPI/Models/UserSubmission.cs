using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumAPI.Models
{
    public class UserSubmission
    {
        [JsonRequired]
        public string Username { get; set; }
        [JsonRequired]
        public string Email { get; set; }

        public UserStatus Status { get; set; }
        public string Password { get; set; }
        public bool HasSignature { get; set; }
        public string Signature { get; set; }
    }
}
