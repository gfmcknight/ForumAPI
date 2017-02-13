using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public string Title { get; set; }
        public DateTime Created { get; set; }

        public int OwnerID { get; set; }
        public Topic Owner { get; set; }
        public int AuthorID { get; set; }
        public User Author { get; set; }
        

        public ICollection<Post> Posts { get; set; }
    }
}
