using ForumAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumAPI.Utilities
{
    /// <summary>
    /// A model that can be serialized into and deserialized into a JWT
    /// payload.
    /// </summary>
    public class JWTBody
    {
        [JsonRequired]
        public string Issuer { get; set; }
        [JsonRequired]
        public string Audience { get; set; }
        [JsonRequired]
        public DateTime Expires { get; set; }
        [JsonRequired]
        public int UserID { get; set; }
        [JsonRequired]
        public UserStatus Status { get; set; }
    }
}
