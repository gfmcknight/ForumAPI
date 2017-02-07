using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumAPI.Models
{
    /// <summary>
    /// Model to store an image as a profile picture or attachment with a post.
    /// Can be used by multiple posts and users without the need for additional
    /// uploads.
    /// </summary>
    public class Picture
    {
        public int ID { get; set; }
        public byte[] Content { get; set; }
        public string Caption { get; set; }
    }
}
