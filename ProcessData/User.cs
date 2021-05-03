using System;
using System.Collections.Generic;

namespace ProcessData
{
    public class User
    {
        public int id;

        public string username;

        public string password;

        public string fullname;

        public DateTime createdAt;

        public Post[] posts;

        public Comment[] comments;

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