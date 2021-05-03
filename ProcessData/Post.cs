using System;
using System.Collections.Generic;

namespace ProcessData
{
    public class Post
    {
        public int id;

        public string content;

        public DateTime createdAt;

        public Comment[] comments;  

        public int userId;    

        public User author; 


        public Post()
        {
            this.createdAt = DateTime.Now;
        }

        public Post(int id, string content, DateTime createdAt, int userId)
        {
            this.id = id;
            this.content = content;
            this.createdAt = createdAt;
            this.userId = userId;
        }

        public override string ToString()
        {
            return $"{id}) |{content}| [{createdAt.ToString()}]";
        }
    }
}