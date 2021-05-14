using System;
using System.IO.Compression;

using ProcessXml;

namespace ProcessData
{
    public static class ExportAndImportData
    {
        private static string savePath = "../data/exports";
        private static string tempSavePath = "../data/exports/temp";

        private static string postsFileName = "posts.xml";
        private static string commentsFileName = "comments.xml";


        public static bool ExportPostsWithComments(string zipName, Post[] posts, Comment[] comments)
        {
            string postsFilePath = $"{tempSavePath}/{postsFileName}";
            string commentsFilePath = $"{tempSavePath}/{commentsFileName}";
            
            try
            {
                XmlSerialization.Serialize<Post[]>(posts, postsFilePath);

                XmlSerialization.Serialize<Comment[]>(comments, commentsFilePath);

                ZipFile.CreateFromDirectory($"{tempSavePath}", $"{savePath}/{zipName}");
            
                System.IO.File.Delete(postsFilePath);
                System.IO.File.Delete(commentsFilePath);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public static bool ImportPostsWithComments(string zipName, UserRepository userRepository, PostRepository postRepository, CommentRepository commentRepository)
        {
            Post[] posts = null;
            Comment[] comments = null;

            try
            {
                ZipFile.ExtractToDirectory($"{savePath}/{zipName}", $"{tempSavePath}");

                XmlSerialization.Deserialize<Post[]>(ref posts, $"{tempSavePath}/{postsFileName}");

                XmlSerialization.Deserialize<Comment[]>(ref comments, $"{tempSavePath}/{commentsFileName}");

                System.IO.File.Delete($"{tempSavePath}/{postsFileName}");
                System.IO.File.Delete($"{tempSavePath}/{commentsFileName}");
            }
            catch
            {
                return false;
            }

            for (int i = 0; i < posts.Length; i++)
            {
                if (!postRepository.PostExists(posts[i].id))
                {
                    if (!userRepository.UserExistsById(posts[i].userId))
                    {
                        User user = new User
                        {
                            id = posts[i].userId,
                            username = $"user{posts[i].userId}",
                            password = $"passuser{posts[i].userId}",
                            fullname = "",
                            createdAt = DateTime.Now,
                            imported = true,
                        };

                        userRepository.InsertImport(user);
                    }

                    postRepository.InsertImport(posts[i]);
                }
            }

            for (int i = 0; i < comments.Length; i++)
            {
                if (!commentRepository.CommentExists(comments[i].id))
                {
                    if (!userRepository.UserExistsById(comments[i].userId))
                    {
                        User user = new User
                        {
                            id = comments[i].userId,
                            username = $"user{posts[i].userId}",
                            password = $"passuser{posts[i].userId}",
                            fullname = "",
                            createdAt = DateTime.Now,
                            imported = true,
                        };

                        userRepository.InsertImport(user);
                    }

                    commentRepository.InsertImport(comments[i]);
                }
            }

            return true;
        }
    }
}