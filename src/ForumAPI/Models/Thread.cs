using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ForumAPI.Models
{
    /// <summary>
    /// Represents a "conversation" between users on a given topic. When
    /// created, an initial post by the owner must be given.
    /// </summary>
    public class Thread
    {
        [Key]
        public int ID { get; set; }
        [JsonRequired]
        public string Title { get; set; }
        public DateTime Created { get; set; }
        public bool Locked { get; set; }

        [JsonRequired]
        public int OwnerID { get; set; }
        [JsonIgnore]
        public Topic Owner { get; set; }

        [JsonRequired]
        public int AuthorID { get; set; }
        [JsonIgnore]
        public User Author { get; set; }
        
        [JsonIgnore]
        public ICollection<Post> Posts { get; set; }
    }
}
