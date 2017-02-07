using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumAPI.Models
{
    /// <summary>
    /// Represents a "conversation" between users on a given topic. When
    /// created, an initial post by the owner must be given.
    /// </summary>
    public class Thread
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public DateTime Created { get; set; }

        public Topic Owner { get; set; }
        public int OwnerID { get; set; }
        public User Author { get; set; }
        public int AuthorID { get; set; }

        public ICollection<Post> Posts { get; set; }
    }
}
