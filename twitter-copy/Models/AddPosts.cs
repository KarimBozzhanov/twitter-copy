using System;

namespace twitter_copy.Models
{
    public class AddPosts
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public DateTime TimeOfCreating { get; set; }
        public string postText { get; set; }
    }
}
