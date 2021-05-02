using System;
using Microsoft.Data.Sqlite;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using System.Diagnostics;
using static System.Console;

namespace ConsoleApp
{
    public class ArgumentsProcessor
    {
        static string[] GetUserEnter()
        {
            Write("Enter command: ");

            string enteredText = ReadLine().Trim();

            string[] command = enteredText.Split();

            if (command[0] == "")
            {
                return null;
            }

            return command;
        }

        public static void Run()
        {
            string databasePath = "../data/database.db";
            SqliteConnection connection = new SqliteConnection($"Data Source={databasePath}");

            UserRepository userRep = new UserRepository(connection);

            PostRepository postRep = new PostRepository(connection);

            CommentRepository commentRep = new CommentRepository(connection);

            Service service = new Service(userRep, postRep, commentRep);


            User currentUser = null;

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
                        if (command[0] == "register")
                        {
                            ProcessRegister(command, userRep);
                        }
                        else if (command[0] == "login")
                        {
                            ProcessLogin(command, userRep, currentUser);
                        }
                        else if (command[0] == "generate")
                        {
                            ProcessGenerate(command, userRep, postRep, commentRep, service);
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

        static void ProcessRegister(string[] command, UserRepository userRep)
        {
            if (command.Length < 4)
            {
                throw new ArgumentException("Wrong command input. Command 'register' must have a form: register {username} {password} {fullname}");
            }

            string username = command[1];
            string password = command[2];
            string fullname = String.Join(' ', command, 3, command.Length - 3);

            bool isUserExists = userRep.UserExists(username);

            if (isUserExists)
            {
                WriteLine($"User with username '{username}' already exist");
            }
            else
            {
                User newUser = new User();

                newUser.username = username;

                SHA256 sha256Hash = SHA256.Create();
                string hashPassword = GetHash(sha256Hash, password);
                newUser.password = hashPassword;
                sha256Hash.Dispose();

                newUser.fullname = fullname;


                int insertedId = userRep.Insert(newUser);

                newUser.id = insertedId;

                WriteLine($"User with id {insertedId} created");
            }
        }

        static void ProcessLogin(string[] command, UserRepository userRep, User currentUser)
        {
            if (command.Length != 3)
            {
                throw new ArgumentException("Wrong command input. Command 'login' must have a form: login {username} {password}");
            }

            string username = command[1];
            string password = command[2];

            User user = userRep.GetByUsername(username);

            if (user == null)
            {
                WriteLine($"User with username '{username}' does not exists");
            }
            else
            {
                SHA256 sha256Hash = SHA256.Create();

                if (VerifyHash(sha256Hash, password, user.password))
                {
                    currentUser = user;

                    WriteLine($"User '{currentUser.username}' was authorized");
                }
                else
                {
                    WriteLine("Wrong password");
                }

                sha256Hash.Dispose();
            }
        }

        static void ProcessGenerate(string[] command, UserRepository userRep, PostRepository postRep, CommentRepository commentRep, Service service)
        {
            if (command.Length != 4)
            {
                throw new ArgumentException("Command 'generate' must have a form: generate {category} {count of generated} " +
                "{timeIntervals (in ISO8601 format with separator 'To': XXXX-XX-XXToXXXX-XX-XX)}");
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

                GeneratePosts(command, postRep, service);
            }
            else if (generateCategory == "comments")
            {
                ValidateUsers(userRep);

                GenerateComments(command, commentRep, service);
            }
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
                throw new ArgumentException("Users not exists, so it's impossible to create posts or comments");
            }
        }
    

        // generate {category} {count} {timeIntervals (in format (XX(month)/XX(day)/XXXX(year)) with separator 'To': XX/XX/XXXXToXX/XX/XXXX)}
        private static void GenerateUsers(string[] command, UserRepository userRep)
        {
            int generatedCount = int.Parse(command[2]);

            DateTime[] datesInterval = ParseDateIntervals(command[3]);

            string filePath = "../data/generator/fullnames.txt";

            string[] fullnames = GetFullnamesFromFile(filePath);

            ///////////////////////////////////////

            Stopwatch sw2 = new Stopwatch();
        
            for (int i = 0; i < generatedCount; i++)
            {
                User newUser = new User();

                string generatedUsername = GenerateRandomUsername();

                if (userRep.UserExists(generatedUsername))
                {
                    WriteLine("conc");
                    i--;
                    continue;
                }

                newUser.username = generatedUsername;

                string password = GenerateRandomPassword(newUser.username);
                SHA256 sha256Hash = SHA256.Create();

                newUser.password = GetHash(sha256Hash, password);

                sha256Hash.Dispose();

                newUser.fullname = GenerateRandomFullname(fullnames);

                DateTime createdAt = GenerateRandomDate(datesInterval);

                newUser.createdAt = createdAt;

                userRep.Insert(newUser);
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

        private static string GenerateRandomPassword(string username)
        {
            Random random = new Random();

            string password = $"{username}pass";

            return password;
        }

        private static string GenerateRandomFullname(string[] fullnames)
        {
            Random random = new Random();

            string fullname = fullnames[random.Next(0, fullnames.Length)];

            return fullname;
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


        private static void GeneratePosts(string[] command, PostRepository postRep, Service service)
        {
            int generatedCount = int.Parse(command[2]);

            DateTime[] datesInterval = ParseDateIntervals(command[3]);

            string filePath = "../data/generator/posts.txt";

            string[] posts = GetRowsDataFromFile(filePath);

            int[] usersIds = service.GetAllUsersId();

            ///////////////////////////////////////
        
            for (int i = 0; i < generatedCount; i++)
            {
                Post newPost = new Post();

                string generatedPostContent = GeneratePostContent(posts);

                newPost.content = generatedPostContent;

                DateTime createdAt = GenerateRandomDate(datesInterval);

                newPost.createdAt = createdAt;

                int userId = GetRandomUserId(usersIds);

                newPost.userId = userId;

                postRep.Insert(newPost);
            }
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

        private static string GeneratePostContent(string[] posts)
        {
            Random random = new Random();

            string post = posts[random.Next(0, posts.Length)];

            return post;
        }

        private static int GetRandomUserId(int[] usersIds)
        {
            Random random = new Random();

            int id = usersIds[random.Next(0, usersIds.Length)];

            return id;
        }


        private static void GenerateComments(string[] command, CommentRepository commentRep, Service service)
        {
            int generatedCount = int.Parse(command[2]);

            DateTime[] datesInterval = ParseDateIntervals(command[3]);

            string filePath = "../data/generator/comments.txt";

            string[] comments = GetRowsDataFromFile(filePath);

            int[] usersIds = service.GetAllUsersId();

            int[] postsIds = service.GetAllUsersId();

            ///////////////////////////////////////
        
            for (int i = 0; i < generatedCount; i++)
            {
                Comment newComment = new Comment();

                string generatedCommentContent = GeneratePostContent(comments);

                newComment.content = generatedCommentContent;

                DateTime createdAt = GenerateRandomDate(datesInterval);

                newComment.createdAt = createdAt;

                int userId = GetRandomUserId(usersIds);

                int postId = GetRandomPostId(postsIds);

                newComment.userId = userId;
                newComment.postId = postId;

                commentRep.Insert(newComment);
            }
        }

        private static int GetRandomPostId(int[] postsIds)
        {
            Random random = new Random();

            int id = postsIds[random.Next(0, postsIds.Length)];

            return id;
        }


        private static string GetHash(HashAlgorithm hashAlgorithm, string input)
        {
 
        // Convert the input string to a byte array and compute the hash.
        byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
 
        // Create a new Stringbuilder to collect the bytes
        // and create a string.
        var sBuilder = new StringBuilder();
 
        // Loop through each byte of the hashed data
        // and format each one as a hexadecimal string.
        for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }
 
        // Return the hexadecimal string.
        return sBuilder.ToString();
        }

        private static bool VerifyHash(HashAlgorithm hashAlgorithm, string input, string hash)
        {
            // Hash the input.
            var hashOfInput = GetHash(hashAlgorithm, input);
 
            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
 
            return comparer.Compare(hashOfInput, hash) == 0;
        }
    }
}