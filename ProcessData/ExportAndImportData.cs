using System;
using System.IO.Compression;
using System.Collections.Generic;

using System.IO;
//using System.IO.Compression;
using System.Xml;
using System.Xml.Linq;

using ProcessXml;
using ScottPlot;



namespace ProcessData
{
    public static class ExportAndImportData
    {
        private static string postsFileName = "posts.xml";
        private static string commentsFileName = "comments.xml";


        public static bool ExportPostsWithComments(string filterValue, string saveDirPath, string zipName,
                                                   PostRepository postsRepository, CommentRepository commentsRepository)
        {
            
            Post[] posts = postsRepository.GetFiltredByTextPosts(filterValue).ToArray();
            Comment[] comments = GetCommentsFromPostsMass(posts, commentsRepository).ToArray();

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
                if (System.IO.File.Exists(tempPostsPath))
                {
                    System.IO.File.Delete(tempPostsPath);
                }
                else if (System.IO.File.Exists(tempCommentsPath))
                {
                     System.IO.File.Delete(tempCommentsPath);
                }
                else if (System.IO.Directory.Exists(tempSaveDirPath))
                {
                    System.IO.Directory.Delete(tempSaveDirPath);
                }

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
    

        private static string GetDirPath(string fullPath)
        {
            string[] fullPathMass = fullPath.Split('/');

            string dirPath = String.Join('/', fullPathMass, 0, fullPathMass.Length - 1);

            return dirPath;
        }



        public static void ExportGraphic(string saveDirPath, string name, DateTime[] dates, int userId,
                                         PostRepository postRepository, CommentRepository commentRepository)
        {
            
            string savePath = $"{saveDirPath}/{name}";

            Post[] posts = postRepository.GetFiltredByTimeUserPosts(userId, dates).ToArray();

            Comment[] comments = GetCommentsFromPostsMass(posts, commentRepository).ToArray();

            DateTime[] datesSpan = GenerateDateSpan(dates);

            string[] datesStrings = GetDatesStrings(datesSpan);

            double[] countOfPosts = GetCountOfPostsByPeriods(userId, postRepository, datesSpan);

            double[] countOfComments = GetCountOfCommentsByPeriods(userId, commentRepository, datesSpan);


            var plt = new ScottPlot.Plot(1280, 720);

            plt.Title("Middle value of posts and comments");
            plt.YLabel("Count of posts and comments");
            plt.XLabel("Date ({month}.{year})");

            int pointCount = datesSpan.Length;
            double[] x = DataGen.Consecutive(pointCount);
            
            plt.PlotScatter(x, countOfPosts);
            plt.PlotScatter(x, countOfComments);
            plt.Ticks(dateTimeX : true);
            plt.XTicks(datesStrings);

            plt.SaveFig(savePath);
        }

        private static DateTime[] GenerateDateSpan(DateTime[] limDates)
        {
            List<DateTime> list = new List<DateTime>();

            int countOfMonths = (int)((limDates[1] - limDates[0]).TotalDays / 30);

            for (int i = 0; i < countOfMonths; i++)
            {
                DateTime date = new DateTime(limDates[0].Year, limDates[0].Month, limDates[0].Day);

                date = date.AddMonths(i);

                list.Add(date);
            }

            return list.ToArray();
        }

        private static string[] GetDatesStrings(DateTime[] dates)
        {
            List<string> list = new List<string>();

            for (int i = 0; i < dates.Length; i++)
            {
                list.Add(dates[i].Month.ToString() + "." + dates[i].Year.ToString());
            }

            return list.ToArray();
        }

        private static double[] GetCountOfPostsByPeriods(int userId, PostRepository postRep, DateTime[] dates)
        {
            double[] count = new double[dates.Length];

            for (int i = 0; i < count.Length; i++)
            {
                DateTime[] checkDates = new DateTime[2];
                checkDates[0] = dates[i];

                if (i == count.Length - 1)
                {
                    DateTime newRightDate = new DateTime(dates[i].Year, dates[i].Month, dates[i].Day);
                    
                    newRightDate = newRightDate.AddMonths(1);
                    
                    checkDates[1] = newRightDate;
                }
                else
                {
                    checkDates[1] = dates[i + 1];
                }

                count[i] = postRep.GetFiltredByTimeUserPosts(userId, checkDates).Count;
            }

            return count;
        }

        private static double[] GetCountOfCommentsByPeriods(int userId, CommentRepository commentRep, DateTime[] dates)
        {
            double[] count = new double[dates.Length];

            for (int i = 0; i < count.Length; i++)
            {
                DateTime[] checkDates = new DateTime[2];
                checkDates[0] = dates[i];

                if (i == count.Length - 1)
                {
                    DateTime newRightDate = new DateTime(dates[i].Year, dates[i].Month, dates[i].Day);
                    
                    newRightDate = newRightDate.AddMonths(1);
                    
                    checkDates[1] = newRightDate;
                }
                else
                {
                    checkDates[1] = dates[i + 1];
                }

                count[i] = commentRep.GetFiltredByTimeUserComments(userId, checkDates).Count;
            }

            return count;
        }


        public static void ExportReport(string saveDirPath, string name, DateTime[] dates,
                                        PostRepository postRepository, CommentRepository commentRepository)
        {
            string blankPath = "../data/generator/report.docx";
            
            string savePath = $"{saveDirPath}/{name}";

            List<Post> posts = postRepository.GetPostsByTimePeriod(dates);

            List<Comment> comments = commentRepository.GetCommentsByTimePeriod(dates);

            Post maxPost = FindPostWithMaxComments(posts, commentRepository);

            string tempOutPath = $"{saveDirPath}/tempDocxExtract";

            ZipFile.ExtractToDirectory(blankPath, tempOutPath);

            XElement root = XElement.Load($"{tempOutPath}/word/document.xml");
            Dictionary<string, string> dict = new Dictionary<string, string>
            {
                {"start", dates[0].ToString()},
                {"finish", dates[1].ToString()},
                {"post", "17"},
                {"comment", "10"},
                {"postMost", "xueta blyadskaya"}
            };
            FindAndReplace(root, dict);
            root.Save($"{tempOutPath}/word/document.xml");
            //File.Delete(@"../data/output/word/media/image1.jpeg");
            //GraphCreator.CreateGraph(d, reportData.user_id, postRepository, commentRepository);
            ZipFile.CreateFromDirectory($"{saveDirPath}/tempDocxExtract", $"{saveDirPath}/{name}");
            Directory.Delete(tempOutPath, true);
        }

        private static void FindAndReplace(XElement node, Dictionary<string, string> dict)
        {
            if (node.FirstNode != null
                && node.FirstNode.NodeType == XmlNodeType.Text)
            {
                string replacement;
                if (dict.TryGetValue(node.Value, out replacement))
                {
                    node.Value = replacement;
                }
            }
            foreach (XElement el in node.Elements())
            {
                FindAndReplace(el, dict);
            }
        }



        public static Post FindPostWithMaxComments(List<Post> posts, CommentRepository commentRepository)
        {
            Post post = null;

            int maxCommentCount = 0;

            for (int i = 0; i < posts.Count; i++)
            {
                List<Comment> commentsOfPost = commentRepository.GetByPostId(posts[i].id);

                if (commentsOfPost.Count > maxCommentCount)
                {
                    maxCommentCount = commentsOfPost.Count;

                    post = posts[i];
                    post.comments = commentsOfPost.ToArray();
                }
            }

            return post;
        }



        private static List<Comment> GetCommentsFromPostsMass(Post[] posts, CommentRepository commentRepository)
        {
            List<Comment> list = new List<Comment>();

            for (int i = 0; i < posts.Length; i++)
            {   
                List<Comment> commentsList = commentRepository.GetByPostId(posts[i].id);

                if (commentsList.Count != 0)
                {
                    for (int j = 0; j < commentsList.Count; j++)
                    {
                        list.Add(commentsList[j]);
                    }
                }
            }

            return list;
        }
    }
}