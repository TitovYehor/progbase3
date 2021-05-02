using System;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace ConsoleApp
{
    public class PostRepository
    {
        private SqliteConnection connection;

        public PostRepository(SqliteConnection connection)
        {
            this.connection = connection;
        }



        public Post[] GetAllPosts()
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM posts";

            SqliteDataReader reader = command.ExecuteReader();

            Post[] posts = ReadPosts(reader);

            reader.Close();

            connection.Close();

            return posts;
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

        public Post[] GetByUserId(int userId) 
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM posts WHERE user_id = $userId";
            command.Parameters.AddWithValue("$userId", userId);

            SqliteDataReader reader = command.ExecuteReader();

            Post[] posts = ReadPosts(reader);

            reader.Close();

            connection.Close();

            return posts;
        }

        public bool Update(int postId, Post post)
        {
            connection.Open();  

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"UPDATE posts
                                    SET content = $content
                                    WHERE id = $postId";
            command.Parameters.AddWithValue("$postId", postId);
            command.Parameters.AddWithValue("$content", post.content);

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
        


        private static Post[] ReadPosts(SqliteDataReader reader)
        {
            List<Post> postsList = new List<Post>();

            while (reader.Read())
            {
                Post post = ReadPost(reader);

                postsList.Add(post);
            }
        
            Post[] posts = new Post[postsList.Count];

            postsList.CopyTo(posts);

            return posts;
        }

        private static Post ReadPost(SqliteDataReader reader)
        {
            int postId = reader.GetInt32(0);
            string content = reader.GetString(1);
            DateTime createdAt = reader.GetDateTime(2);
            int userId = reader.GetInt32(3);

            Post post = new Post(postId, content, createdAt, userId);

            return post;
        }
    }
}