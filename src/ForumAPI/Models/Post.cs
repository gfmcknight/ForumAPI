using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ForumAPI.Models
{
    /// <summary>
    /// The basic unit of communication in a forum. Belongs to a user and a
    /// thread in which the post occurs.
    /// </summary>
    public class Post
    {
        public int ID { get; set; }
        public string Text { get; set; }
        public DateTime Created { get; set; }

        // The Signature and HasSignature properties are copied from the user
        // at the time of post creation, so that when the user changes their
        // signature, old signatures remain.
        public bool HasSignature { get; set; }
        public string Signature { get; set; }

        public int OwnerID { get; set; }
        [ForeignKey("OwnerID")] [JsonIgnore]
        public Thread Owner { get; set; }

        public int AuthorID { get; set; }
        [ForeignKey("AuthorID")] [JsonIgnore]
        public User Author { get; set; }
        
    }
}
