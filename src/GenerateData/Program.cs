using System;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using static System.Console;

using ProcessData;

namespace GenerateData
{
    class GenerateData
    {
        static string[] GetUserEnter()
        {
            Write("Enter command ('help' to help): ");

            string enteredText = ReadLine().Trim();

            string[] command = enteredText.Split();

            if (command[0] == "")
            {
                return null;
            }

            return command;
        }

        static void Main(string[] args)
        {
            string databasePath = "../data/database.db";

            UserRepository userRep = new UserRepository(databasePath);

            PostRepository postRep = new PostRepository(databasePath);

            CommentRepository commentRep = new CommentRepository(databasePath);

            while (true)
            {
                WriteLine("".PadRight(40, '='));

                string[] command = GetUserEnter();

                WriteLine("".PadRight(40, '-'));

                if (command == null || (command[0] == "exit" && command.Length == 1))
                {
                    WriteLine("Program ending");

                    WriteLine("".PadRight(40, '='));

                    break;
                }

                if (command[0] == "exit")
                {
                    Error.WriteLine("Wrong command input. Command must consist of at least 2 elements ('exit' or '' to exit)");
                }
                else
                {
                    try 
                    {
                        if (command[0] == "generate")
                        {
                            ProcessGenerate(command, userRep, postRep, commentRep);
                        }
                        else
                        {
                            Error.WriteLine($"Input error. Unknown command '{command[0]}'");
                        }
                    }
                    catch (Exception ex)
                    {
                        Error.WriteLine($"[Error] - {ex}");
                    }
                }
            }
        }

        static void ProcessGenerate(string[] command, UserRepository userRep, PostRepository postRep, CommentRepository commentRep)
        {
            if (command.Length < 4)
            {
                throw new ArgumentException("Command 'generate' must have a form: generate {category} {count of generated} " +
                "{timeIntervals (in format (XX(month)/XX(day)/XXXX(year)) with separator 'To': XX/XX/XXXXToXX/XX/XXXX)}");
            }

            string generateCategory = command[1];

            ValidateGenerateCategory(generateCategory);

            ValidateGeneratedCount(command[2]);

            if (generateCategory == "users")
            {
                GenerateUsers(command, userRep);
            }
            else if (generateCategory == "posts")
            {
                ValidateUsers(userRep);

                GeneratePosts(command, userRep, postRep, commentRep);
            }
            else if (generateCategory == "comments")
            {
                ValidateUsers(userRep);

                GenerateComments(command, userRep, postRep, commentRep);
            } 

            WriteLine($"{command[2]} {generateCategory} successfully generated");
        }
    
        private static void ValidateGenerateCategory(string category)
        {
            string[] supportedGenerateCategories = new string[]{"users", "posts", "comments"};

            for (int i = 0; i < supportedGenerateCategories.Length; i++)
            {
                if (supportedGenerateCategories[i] == category)
                {
                    return;
                }
            }

            throw new ArgumentException($"Unsupported generating category '{category}' ('help' - to see supported categories)");
        }

        private static void ValidateGeneratedCount(string enteredCount)
        {
            int generatedCount;
            bool tryCount = int.TryParse(enteredCount, out generatedCount);
            if (!tryCount || generatedCount <= 0)
            {
                throw new ArgumentException("Incorrect count of generating entities ('help' to help)");
            }
        }

        private static void ValidateUsers(UserRepository userRep)
        {
            User[] users = userRep.GetAllUsers();

            if (users.Length == 0)
            {
                throw new ArgumentException("Users does not exists, so it's impossible to create posts or comments");
            }
        }
    

        
        private static void GenerateUsers(string[] command, UserRepository userRep)
        {
            ValidateUsersRole(command[4]);

            string usersRole = command[4];

            int generatedCount = int.Parse(command[2]);

            DateTime[] datesInterval = ParseDateIntervals(command[3]);

            string filePath = "../data/generator/fullnames.txt";

            string[] fullnames = GetFullnamesFromFile(filePath);

            Authentication authentication = new Authentication(userRep);

            ///////////////////////////////////////
        
            for (int i = 0; i < generatedCount; i++)
            {
                User newUser = new User();

                string generatedUsername = GenerateRandomUsername();

                newUser.username = generatedUsername;

                newUser.password = GenerateRandomHashPassword(newUser.username); 

                newUser.fullname = GetRandomElementFromMass<string>(fullnames);

                newUser.createdAt = GenerateRandomDate(datesInterval);

                newUser.role = usersRole;

                if (!authentication.Register(newUser))
                {
                    i--;
                    continue;
                }
            }
        }

        private static void ValidateUsersRole(string enteredRole)
        {
            string[] roles = new string[]{"user", "moderator", "admin"};

            for (int i = 0; i < roles.Length; i++)
            {
                if (enteredRole == roles[i])
                {
                    return;
                }
            }

            throw new ArgumentException($"Invalid entered role - '{enteredRole}' ('help' to help)");
                
        }

        private static void GeneratePosts(string[] command, UserRepository userRep, PostRepository postRep, CommentRepository commentRep)
        {
            int generatedCount = int.Parse(command[2]);

            DateTime[] datesInterval = ParseDateIntervals(command[3]);

            string filePath = "../data/generator/posts.txt";
            string[] posts = GetRowsDataFromFile(filePath);

            int[] usersIds = userRep.GetAllUsersIds();

            int[] commentsIds = commentRep.GetAllCommentsIds();

            ///////////////////////////////////////
        
            for (int i = 0; i < generatedCount; i++)
            {
                Post newPost = new Post();

                newPost.content = GetRandomElementFromMass<string>(posts);

                newPost.createdAt = GenerateRandomDate(datesInterval);

                newPost.userId = GetRandomElementFromMass<int>(usersIds);

                postRep.Insert(newPost);
            }
        }

        private static void GenerateComments(string[] command, UserRepository userRep, PostRepository postRep, CommentRepository commentRep)
        {
            int generatedCount = int.Parse(command[2]);

            DateTime[] datesInterval = ParseDateIntervals(command[3]);

            string filePath = "../data/generator/comments.txt";

            string[] comments = GetRowsDataFromFile(filePath);

            int[] usersIds = userRep.GetAllUsersIds();

            int[] postsIds = postRep.GetAllPostsIds();

            ///////////////////////////////////////
        
            for (int i = 0; i < generatedCount; i++)
            {
                Comment newComment = new Comment();

                newComment.content = GetRandomElementFromMass<string>(comments);

                newComment.createdAt = GenerateRandomDate(datesInterval);

                newComment.userId = GetRandomElementFromMass<int>(usersIds);
                newComment.postId = GetRandomElementFromMass<int>(postsIds);

                commentRep.Insert(newComment);
            }
        }



        private static string[] GetFullnamesFromFile(string filePath)
        {
            List<string> namesList = new List<string>();

            StreamReader sr = new StreamReader(filePath);

            for (int i = 0; i < 500; i++)
            {
                string row = sr.ReadLine();

                namesList.Add(row);
            }

            sr.Close();

            string[] fullnames = new string[namesList.Count];

            namesList.CopyTo(fullnames);

            return fullnames;
        }

        private static string GenerateRandomUsername()
        {
            Random random = new Random();

            string username = "";

            for (int i = 0; i < 11; i++)
            {
                if (i % 2 == 0)
                {
                    username += (char)random.Next(48, 58);
                }   
                else if (i % 3 == 0)
                {
                    username += (char)random.Next(65, 91);
                }
                else
                {
                    username += (char)random.Next(97, 123);
                }
            }

            return username;
        }

        private static string GenerateRandomHashPassword(string username)
        {
            Random random = new Random();

            string password = $"{username}pass";

            return password;
        }


        private static DateTime[] ParseDateIntervals(string dateIntervals)
        {
            string[] splittedDates = dateIntervals.Split("To");

            DateTime leftLimDate;
            DateTime rightLimDate;

            bool leftDateParsed = DateTime.TryParseExact(splittedDates[0], "d", CultureInfo.InvariantCulture, DateTimeStyles.None, out leftLimDate);
            bool rightLimParsed = DateTime.TryParseExact(splittedDates[1], "d", CultureInfo.InvariantCulture, DateTimeStyles.None, out rightLimDate);

            if (!leftDateParsed || !rightLimParsed)
            {
                throw new ArgumentException("Incorrect date format ('help' to help)");
            }

            int daysInterval = (rightLimDate - leftLimDate).Days;

            if (daysInterval <= 0)
            {
                throw new ArgumentException("Incorrect dates interval ('help' to help)");
            }

            DateTime[] parsedDates = new DateTime[]{leftLimDate, rightLimDate};

            return parsedDates;
        }

        private static DateTime GenerateRandomDate(DateTime[] datesInterval)
        {
            DateTime leftLimDate = datesInterval[0];
            DateTime rightLimDate = datesInterval[1];

            Random random = new Random();

            int daysDiff = (rightLimDate - leftLimDate).Days;

            DateTime generatedDateTime = leftLimDate.AddDays(random.Next(0, daysDiff + 1)).AddHours(random.Next(0, 24)).AddMinutes(random.Next(0, 60)).AddSeconds(random.Next(0, 60)).AddMilliseconds(random.Next(0, 1000));

            return generatedDateTime;
        }


        private static string[] GetRowsDataFromFile(string filePath)
        {
            List<string> rowsList = new List<string>();

            StreamReader sr = new StreamReader(filePath);

            for (int i = 0; i < 30; i++)
            {
                string row = sr.ReadLine();

                rowsList.Add(row);
            }

            sr.Close();
            
            string[] rows = new string[rowsList.Count];

            rowsList.CopyTo(rows);

            return rows;
        }

        private static T GetRandomElementFromMass<T>(T[] mass)
        {
            Random random = new Random();

            T randomElement = mass[random.Next(0, mass.Length)];

            return randomElement;
        }
    }
}
