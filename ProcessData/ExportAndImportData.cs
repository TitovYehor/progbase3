using System;
using System.IO.Compression;
using System.Collections.Generic;

using ProcessXml;

namespace ProcessData
{
    public static class ExportAndImportData
    {
        //private static string savePath = "../data/exports";
        //private static string tempSavePath = "../data/exports/temp";

        private static string postsFileName = "posts.xml";
        private static string commentsFileName = "comments.xml";


        public static bool ExportPostsWithComments(string filterValue, string saveDirPath, string zipName, PostRepository postsRepository)
        {
            Post[] posts = postsRepository.GetFiltredByTextPosts(filterValue);
            Comment[] comments = GetCommentsFromPostsMass(posts);

            string tempSavePath = $"{saveDirPath}/tempExportSave";

            string postsFilePath = $"{tempSavePath}/{postsFileName}";
            string commentsFilePath = $"{tempSavePath}/{commentsFileName}";
            
            try
            {
                System.IO.Directory.CreateDirectory(tempSavePath);

                XmlSerialization.Serialize<Post[]>(posts, postsFilePath);

                XmlSerialization.Serialize<Comment[]>(comments, commentsFilePath);

                ZipFile.CreateFromDirectory(tempSavePath, $"{saveDirPath}/{zipName}");
            
                System.IO.File.Delete(postsFilePath);
                System.IO.File.Delete(commentsFilePath);
                System.IO.Directory.Delete(tempSavePath);
            }
            catch
            {
                if (System.IO.File.Exists(postsFilePath))
                {
                    System.IO.File.Delete(postsFilePath);
                }
                else if (System.IO.File.Exists(commentsFilePath))
                {
                     System.IO.File.Delete(commentsFilePath);
                }
                else if (System.IO.Directory.Exists(tempSavePath))
                {
                    System.IO.Directory.Delete(tempSavePath);
                }

                return false;
            }

            return true;
        }

        public static bool ImportPostsWithComments(string zipPath, UserRepository userRepository, PostRepository postRepository, CommentRepository commentRepository)
        {
            string saveDirPath = GetDirPath(zipPath);
            string tempSaveDirPath = $"{saveDirPath}/tempImportSave";

            string tempPostsPath = $"{tempSaveDirPath}/{postsFileName}";
            string tempCommentsPath = $"{tempSaveDirPath}/{commentsFileName}";

            Post[] posts = null;
            Comment[] comments = null;

            try
            {
                System.IO.Directory.CreateDirectory(tempSaveDirPath);
                ZipFile.ExtractToDirectory(zipPath, tempSaveDirPath);

                XmlSerialization.Deserialize<Post[]>(ref posts, tempPostsPath);

                XmlSerialization.Deserialize<Comment[]>(ref comments, tempCommentsPath);

                System.IO.File.Delete(tempPostsPath);
                System.IO.File.Delete(tempCommentsPath);
                System.IO.Directory.Delete(tempSaveDirPath);
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
    

        private static Comment[] GetCommentsFromPostsMass(Post[] posts)
        {
            List<Comment> list = new List<Comment>();

            if (posts.Length == 0)
            {
                return null;
            }
            else
            {
                for (int i = 0; i < posts.Length; i++)
                {
                    for (int j = 0; j < posts[i].comments.Length; j++)
                    {
                        list.Add(posts[i].comments[j]);
                    }
                }

                Comment[] comments = new Comment[list.Count];
                list.CopyTo(comments);

                return comments;
            }
        }


        private static string GetDirPath(string fullPath)
        {
            string[] fullPathMass = fullPath.Split('/');

            string dirPath = String.Join('/', fullPathMass, 0, fullPathMass.Length - 1);

            return dirPath;
        }
    }
}