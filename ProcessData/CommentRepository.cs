using System;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace ProcessData
{
    public class CommentRepository
    {
        private SqliteConnection connection;


        public CommentRepository(string databasePath)
        {
            SqliteConnection connection = new SqliteConnection($"Data Source={databasePath}");
            
            this.connection = connection;
        }


        public int GetSearchPagesCount(int pageSize, string searchValue)
        {
            if (pageSize < 1)
            {
                throw new ArgumentOutOfRangeException($"Page size can not be '{pageSize}'");
            }

            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM comments 
                                    WHERE content LIKE '%' || $searchValue || '%'";
            command.Parameters.AddWithValue("$searchValue", searchValue);

            int totalFound = (int)(long)command.ExecuteScalar();

            connection.Close();

            int totalSearchPages = (int)Math.Ceiling((float)totalFound / (float)pageSize);

            return totalSearchPages;
        }

        public List<Comment> GetSearchPage(string searchValue, int pageNum, int pageSize)
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

            command.CommandText = @"SELECT * FROM comments 
                                    WHERE content LIKE '%' || $searchValue || '%'
                                    LIMIT $skip,$countOfOut";
            command.Parameters.AddWithValue("$searchValue", searchValue);
            command.Parameters.AddWithValue("$skip", (pageNum - 1) * pageSize);
            command.Parameters.AddWithValue("$countOfOut", pageSize);

            SqliteDataReader reader = command.ExecuteReader();

            List<Comment> searchPage = ReadComments(reader);

            reader.Close();

            connection.Close();

            return searchPage;
        }
        
        public int Insert(Comment comment) 
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();

            command.CommandText = 
            @"
                INSERT INTO comments (content, createdAt, user_id, post_id) 
                VALUES ($content, $createdAt, $userId, $postId);

                SELECT last_insert_rowid();
            ";

            command.Parameters.AddWithValue("$content", comment.content);
            command.Parameters.AddWithValue("$createdAt", comment.createdAt.ToString("o"));
            command.Parameters.AddWithValue("$userId", comment.userId);
            command.Parameters.AddWithValue("$postId", comment.postId);

            int insertedId = (int)(long)command.ExecuteScalar();

            connection.Close();

            return insertedId;
        }

        public int InsertImport(Comment comment) 
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();

            command.CommandText = 
            @"
                INSERT INTO comments (id, content, createdAt, user_id, post_id) 
                VALUES ($id, $content, $createdAt, $userId, $postId);

                SELECT last_insert_rowid();
            ";

            command.Parameters.AddWithValue("$id", comment.id);
            command.Parameters.AddWithValue("$content", comment.content);
            command.Parameters.AddWithValue("$createdAt", comment.createdAt.ToString("o"));
            command.Parameters.AddWithValue("$userId", comment.userId);
            command.Parameters.AddWithValue("$postId", comment.postId);

            int insertedId = (int)(long)command.ExecuteScalar();

            connection.Close();

            return insertedId;
        }

        public bool CommentExists(int id)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM comments WHERE id = $id";
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

        public Comment[] GetByUserId(int userId) 
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM comments WHERE user_id = $userId";
            command.Parameters.AddWithValue("$user_Id", userId);

            SqliteDataReader reader = command.ExecuteReader();

            List<Comment> list = ReadComments(reader);

            reader.Close();

            connection.Close();

            Comment[] comments = new Comment[list.Count];
            list.CopyTo(comments);

            return comments;
        }

        public Comment[] GetByPostId(int postId) 
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM comments WHERE post_id = $postId";
            command.Parameters.AddWithValue("$post_id", postId);

            SqliteDataReader reader = command.ExecuteReader();

            List<Comment> list = ReadComments(reader);

            reader.Close();

            connection.Close();

            Comment[] comments = new Comment[list.Count];
            list.CopyTo(comments);

            return comments;
        }

        public bool Update(int commentId, Comment comment)
        {
            connection.Open();  

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"UPDATE comments
                                    SET content = $content
                                    WHERE id = $commentId";
            command.Parameters.AddWithValue("$commentId", commentId);
            command.Parameters.AddWithValue("$content", comment.content);

            int nChanged = command.ExecuteNonQuery();

            bool isUpdated = false;

            if (nChanged != 0)
            {
                isUpdated = true;
            }

            connection.Close();

            return isUpdated;
        }

        public bool DeleteById(int commentId)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM comments WHERE id = $id";
            command.Parameters.AddWithValue("$id", commentId);

            int deletedCount = command.ExecuteNonQuery();

            bool isDeleted = false;

            if (deletedCount != 0)
            {
                isDeleted = true;
            }

            connection.Close();

            return isDeleted;
        }



        private static List<Comment> ReadComments(SqliteDataReader reader)
        {
            List<Comment> commentsList = new List<Comment>();

            while (reader.Read())
            {
                Comment comment = ReadComment(reader);

                commentsList.Add(comment);
            }

            return commentsList;
        }

        private static Comment ReadComment(SqliteDataReader reader)
        {
            int commentId = reader.GetInt32(0);
            string content = reader.GetString(1);
            DateTime createdAt = reader.GetDateTime(2);
            int authorId = reader.GetInt32(3);
            int postId = reader.GetInt32(4);
            
            Comment comment = new Comment(commentId, content, createdAt, authorId, postId);

            return comment;
        }
    }
}