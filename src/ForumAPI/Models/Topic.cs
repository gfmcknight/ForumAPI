using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForumAPI.Models
{
    public class Topic
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }

        
        public int ParentID { get; set; }
        [ForeignKey("ParentID")]
        public Topic Parent { get; set; }
        
        public ICollection<Topic> Topics { get; set; }
        public ICollection<Thread> Threads { get; set; }
    }
}
