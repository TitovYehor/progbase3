using System;
using System.Collections.Generic;

namespace ConsoleApp
{
    public class User
    {
        public int id;

        public string username;

        public string password;

        public string fullname;

        public DateTime createdAt;

        public List<Post> posts;

        public List<Comment> comments;

        public User()
        {
            this.createdAt = DateTime.Now;
        }

        public User(int id, string username, string password, string fullname, DateTime createdAt)
        {
            this.id = id;
            this.username = username;
            this.password = password;
            this.fullname = fullname;
            this.createdAt = createdAt;
        }
    }
}