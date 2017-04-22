using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace ForumAPI.Models
{
    /// <summary>
    /// Represents a section of the forum to which many different threads
    /// can be posted, as well as subtopics.
    /// </summary>
    public class Topic
    {
        [Key]
        public int ID { get; set; }

        [JsonRequired]
        public string Name { get; set; }
        [JsonRequired]
        public string Description { get; set; }
        [JsonRequired]
        public bool AllowsThreads { get; set; }
        
        [InverseProperty("Child")]
        [JsonIgnore]
        public ICollection<TopicRelation> Parents { get; set; }

        [InverseProperty("Parent")]
        [JsonIgnore]
        public ICollection<TopicRelation> SubTopics { get; set; }

        [InverseProperty("Owner")] [JsonIgnore]
        public ICollection<Thread> Threads { get; set; }
    }
}
