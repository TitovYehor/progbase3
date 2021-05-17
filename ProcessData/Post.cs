using System;
using System.Xml.Serialization;

namespace ProcessData
{
    public class Post
    {
        public int id;

        public string content;

        public DateTime createdAt;
        
        [XmlIgnore]
        public Comment[] comments;  
        
        public int userId;   

        [XmlIgnore]
        public User author;

        public int? pinComment;


        public Post()
        {
            this.createdAt = DateTime.Now;
            
            this.comments = new Comment[0];
        }

        public Post(int id, string content, DateTime createdAt, int userId, int? pinComment)
        {
            this.id = id;
            this.content = content;
            this.createdAt = createdAt;
            this.userId = userId;
            this.pinComment = pinComment;

            this.comments = new Comment[0];
        }


        public bool ComparePostsIds(Post post)
        {
            if (id == post.id)
            {
                return true;
            }

            return false;
        }


        public override string ToString()
        {
            return $"{id}) |{content}| [{createdAt.ToString()}]";
        }
    }
}