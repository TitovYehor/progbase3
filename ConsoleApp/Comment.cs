using System;

namespace ConsoleApp
{
    public class Comment
    {   
        public int id;
        
        public string content;

        public DateTime createdAt;

        public int postId;

        public int userId;

        public User author;


        public Comment()
        {
            this.createdAt = DateTime.Now;
        }

        public Comment(int id, string content, DateTime createdAt, int userId, int postId)
        {
            this.id = id;
            this.content = content;
            this.createdAt = createdAt;
            this.userId = userId;
            this.postId = postId;
        }
    }
}