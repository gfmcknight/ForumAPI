using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace ForumAPI.Models
{
    public class TopicRelation
    {
        [Key]
        public int ID { get; set; }

        [JsonRequired]
        public Topic Parent { get; set; }

        [JsonRequired]
        public Topic Child { get; set; }
    }
}
