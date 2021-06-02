using System;
using System.Linq;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace ProcessData
{
    public class PostRepository
    {
        private SqliteConnection connection;

        public PostRepository(string databasePath)
        {
            SqliteConnection connection = new SqliteConnection($"Data Source={databasePath}");
            
            this.connection = connection;
        }



        public Post[] GetAllPosts()
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM posts";

            SqliteDataReader reader = command.ExecuteReader();

            List<Post> list = ReadPosts(reader);
            
            reader.Close();

            connection.Close();

            Post[] posts = new Post[list.Count];
            list.CopyTo(posts);

            return posts;
        }

        public int[] GetAllPostsIds()
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT id FROM posts";

            SqliteDataReader reader = command.ExecuteReader();

            List<int> idsList = GetListOfIds(reader);

            reader.Close();

            connection.Close();

            int[] ids = new int[idsList.Count];
            idsList.CopyTo(ids);

            return ids;
        }

        public bool PostExists(int id)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM posts WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);

            int countOfFound = (int)(long)command.ExecuteScalar();

            bool isExists = false;

            if (countOfFound != 0)
            {
                isExists = true;
            }

            connection.Close();

            return isExists;
        }


        public int GetSearchPagesCount(int pageSize, string searchValue)
        {
            if (pageSize < 1)
            {
                throw new ArgumentOutOfRangeException($"Page size can not be '{pageSize}'");
            }

            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM posts 
                                    WHERE content LIKE '%' || $searchValue || '%'";
            command.Parameters.AddWithValue("$searchValue", searchValue);

            int totalFound = (int)(long)command.ExecuteScalar();

            connection.Close();

            int totalSearchPages = (int)Math.Ceiling((float)totalFound / (float)pageSize);

            return totalSearchPages;
        }
        public List<Post> GetSearchPage(string searchValue, int pageNum, int pageSize)
        {
            if (pageNum < 1)
            {
                throw new ArgumentOutOfRangeException($"Page '{pageNum}' out of range");
            }

            if (pageSize < 1)
            {
                throw new ArgumentOutOfRangeException($"Page size can not be '{pageSize}'");
            }

            connection.Open();

            SqliteCommand command = connection.CreateCommand();

            command.CommandText = @"SELECT * FROM posts 
                                    WHERE content LIKE '%' || $searchValue || '%'
                                    LIMIT $skip,$countOfOut";
            command.Parameters.AddWithValue("$searchValue", searchValue);
            command.Parameters.AddWithValue("$skip", (pageNum - 1) * pageSize);
            command.Parameters.AddWithValue("$countOfOut", pageSize);

            SqliteDataReader reader = command.ExecuteReader();

            List<Post> searchPage = ReadPosts(reader);

            reader.Close();

            connection.Close();

            return searchPage;
        }

        public List<Post> GetSearchUserPostsPage(int userId, string searchValue, int pageNum, int pageSize)
        {
            if (pageNum < 1)
            {
                throw new ArgumentOutOfRangeException($"Page '{pageNum}' out of range");
            }

            if (pageSize < 1)
            {
                throw new ArgumentOutOfRangeException($"Page size can not be '{pageSize}'");
            }

            connection.Open();

            SqliteCommand command = connection.CreateCommand();

            command.CommandText = @"SELECT * FROM posts 
                                    WHERE content LIKE '%' || $searchValue || '%' AND user_id = $userId
                                    LIMIT $skip,$countOfOut";
            command.Parameters.AddWithValue("$userId", userId);
            command.Parameters.AddWithValue("$searchValue", searchValue);
            command.Parameters.AddWithValue("$skip", (pageNum - 1) * pageSize);
            command.Parameters.AddWithValue("$countOfOut", pageSize);

            SqliteDataReader reader = command.ExecuteReader();

            List<Post> searchPage = ReadPosts(reader);

            reader.Close();

            connection.Close();

            return searchPage;
        }
        public int GetSearchUserPostsPagesCount(int userId, int pageSize, string searchValue)
        {
            if (pageSize < 1)
            {
                throw new ArgumentOutOfRangeException($"Page size can not be '{pageSize}'");
            }

            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM posts 
                                    WHERE content LIKE '%' || $searchValue || '%' AND user_id = $userId";
            command.Parameters.AddWithValue("$userId", userId);
            command.Parameters.AddWithValue("$searchValue", searchValue);

            int totalFound = (int)(long)command.ExecuteScalar();

            connection.Close();

            int totalSearchPages = (int)Math.Ceiling((float)totalFound / (float)pageSize);

            return totalSearchPages;
        }


        public int Insert(Post post) 
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();

            command.CommandText = 
            @"
                INSERT INTO posts (content, createdAt, user_id) 
                VALUES ($content, $createdAt, $userId);

                SELECT last_insert_rowid();
            ";

            command.Parameters.AddWithValue("$content", post.content);
            command.Parameters.AddWithValue("$createdAt", post.createdAt.ToString("o"));
            command.Parameters.AddWithValue("$userId", post.userId);

            int insertedId = (int)(long)command.ExecuteScalar();

            connection.Close();

            return insertedId;
        }

        public int InsertImport(Post post) 
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();

            command.CommandText = 
            @"
                INSERT INTO posts (id, content, createdAt, user_id) 
                VALUES ($id, $content, $createdAt, $userId);

                SELECT last_insert_rowid();
            ";

            command.Parameters.AddWithValue("$id", post.id);
            command.Parameters.AddWithValue("$content", post.content);
            command.Parameters.AddWithValue("$createdAt", post.createdAt.ToString("o"));
            command.Parameters.AddWithValue("$userId", post.userId);

            int insertedId = (int)(long)command.ExecuteScalar();

            connection.Close();

            return insertedId;
        }

        public Post[] GetByUserId(int userId) 
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM posts WHERE user_id = $userId";
            command.Parameters.AddWithValue("$userId", userId);

            SqliteDataReader reader = command.ExecuteReader();

            List<Post> list = ReadPosts(reader);

            reader.Close();

            connection.Close();

            Post[] posts = new Post[list.Count];
            list.CopyTo(posts);

            return posts;
        }

        public bool Update(int postId, Post post)
        {
            connection.Open();  

            SqliteCommand command = connection.CreateCommand();

            if (post.pinComment == null)
            {
                command.CommandText = @"UPDATE posts
                                    SET content = $content
                                    WHERE id = $postId";
                command.Parameters.AddWithValue("$postId", postId);
                command.Parameters.AddWithValue("$content", post.content);
            }
            else
            {
                command.CommandText = @"UPDATE posts
                                    SET content = $content,
                                        pinned_comment = $pinComment
                                    WHERE id = $postId";
                command.Parameters.AddWithValue("$postId", postId);
                command.Parameters.AddWithValue("$content", post.content);
                command.Parameters.AddWithValue("$pinComment", post.pinComment);
            }

            int nChanged = command.ExecuteNonQuery();

            bool isUpdated = false;

            if (nChanged != 0)
            {
                isUpdated = true;
            }

            connection.Close();

            return isUpdated;
        }

        public bool DeleteById(int postId)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM posts WHERE id = $id";
            command.Parameters.AddWithValue("$id", postId);

            int deletedCount = command.ExecuteNonQuery();

            bool isDeleted = false;

            if (deletedCount != 0)
            {
                isDeleted = true;
            }

            connection.Close();

            return isDeleted;
        }
        


        public List<Post> GetFiltredByTextPosts(string text)
        {
            // connection.Open();

            // SqliteCommand command = connection.CreateCommand();
            // command.CommandText = @"SELECT * FROM posts, comments 
            //                         WHERE posts.id = comments.post_id 
            //                         AND posts.content LIKE '%' || $valueX || '%'";
            // command.Parameters.AddWithValue("$valueX", text);

            // SqliteDataReader reader = command.ExecuteReader();

            // Post[] posts = ReadPostsFromCrossJoin(reader);

            // reader.Close();

            // connection.Close();

            // return posts;

            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM posts 
                                    WHERE posts.content LIKE '%' || $valueX || '%'";
            command.Parameters.AddWithValue("$valueX", text);

            SqliteDataReader reader = command.ExecuteReader();

            List<Post> posts = ReadPosts(reader);

            connection.Close();

            return posts;
        }

        public Post[] GetFiltredByTimePosts(DateTime[] dateIntervals)
        {
            connection.Open();

            DateTime dateLeft = dateIntervals[0];
            DateTime dateRight = dateIntervals[1];

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM posts, comments 
                                    WHERE posts.id = comments.post_id 
                                    AND posts.createdAt > $leftDateLim AND posts.createdAt < rightDateLim";
            command.Parameters.AddWithValue("$leftDateLim", dateLeft.ToString("o"));
            command.Parameters.AddWithValue("$rightDateLim", dateRight.ToString("o"));

            SqliteDataReader reader = command.ExecuteReader();

            Post[] posts = ReadPostsFromCrossJoin(reader);

            reader.Close();

            connection.Close();

            return posts;
        }



        private static List<Post> ReadPosts(SqliteDataReader reader)
        {
            List<Post> postsList = new List<Post>();

            while (reader.Read())
            {
                Post post = ReadPost(reader);

                postsList.Add(post);
            }

            return postsList;
        }

        private static Post ReadPost(SqliteDataReader reader)
        {
            int postId = reader.GetInt32(0);
            string content = reader.GetString(1);
            DateTime createdAt = reader.GetDateTime(2);
            int userId = reader.GetInt32(3);
            int? pinComment = null;
            if (!reader.IsDBNull(4))
            {
                pinComment = reader.GetInt32(4);
            }
            

            Post post = new Post(postId, content, createdAt, userId, pinComment);

            return post;
        }
    

        public static Post[] ReadPostsFromCrossJoin(SqliteDataReader reader)
        {
            List<Post> postsList = new List<Post>();

            while (reader.Read())
            {
                Comment comment = ReadCommentFromCrossJoin(reader);

                Post post = ReadPost(reader);

                int index = postsList.FindIndex(post.ComparePostsIds);

                if (index == -1)
                {
                    AddElementToMass<Comment>(ref post.comments, comment);

                    postsList.Add(post);
                }
                else
                {
                    AddElementToMass<Comment>(ref postsList[index].comments, comment);
                }
            }

            Post[] posts = new Post[postsList.Count];

            postsList.CopyTo(posts);

            return posts;
        }

        private static Comment ReadCommentFromCrossJoin(SqliteDataReader reader)
        {
            int id = reader.GetInt32(5);
            string content = reader.GetString(6);
            DateTime createdAt = reader.GetDateTime(7);
            int userId = reader.GetInt32(8);
            int postId = reader.GetInt32(9);

            Comment comment = new Comment(id, content, createdAt, userId, postId);

            return comment;
        }
    
        private static void AddElementToMass<T>(ref T[] mass, T element)
        {
            List<T> listOfMass = mass.ToList<T>();

            listOfMass.Add(element);

            mass = new T[listOfMass.Count];

            listOfMass.CopyTo(mass);
        }
    
        private static List<int> GetListOfIds(SqliteDataReader reader)
        {
            List<int> list = new List<int>();

            while (reader.Read())
            {
                list.Add(reader.GetInt32(0));
            } 

            return list;
        }
    }
}