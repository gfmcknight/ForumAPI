using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumAPI.Models
{
    public class Topic
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Topic Parent { get; set; }
        public int ParentID { get; set; }

        public ICollection<Topic> Topics { get; set; }
        public ICollection<Thread> Threads { get; set; }
    }
}
