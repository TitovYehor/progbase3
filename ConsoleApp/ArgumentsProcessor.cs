using System;
using Microsoft.Data.Sqlite;
using System.Security.Cryptography;
using System.Text;
using System.IO;
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

        static void ProcessGenerate(string[] command, UserRepository userRep, PostRepository postRep, CommentRepository commentRep)
        {
            if (command.Length != 4)
            {
                throw new ArgumentException("Command 'generate' must have a form: generate {category} {count of generated} " +
                "{timeIntervals (in ISO8601 format with separator 'To': XXXX-XX-XXToXXXX-XX-XX)}");
            }

            string generateCategory = command[1];

            ValidateGenerateCategory(generateCategory);

            if (generateCategory == "users")
            {
                GenerateUsers(command, userRep);
            }
        }

        private static void ValidateGenerateCategory(string category)
        {
            string[] supportedGenerateCategories = new string[]{"users, posts, comments"};

            for (int i = 0; i < supportedGenerateCategories.Length; i++)
            {
                if (supportedGenerateCategories[i] == category)
                {
                    return;
                }
            }

            throw new ArgumentException($"Unsupported generating category '{category}' ('help' - to see supported categories)");
        }

        // generate users {count} {timeIntervals (in ISO8601 format with separator 'To': XXXX-XX-XXToXXXX-XX-XX)}
        private static void GenerateUsers(string[] command, UserRepository userRep)
        {
            ValidateGeneratedCount(command[2]);

            int generatedCount = int.Parse(command[2]);

            string[] datesInterval = ParseDateIntervals(command[3]);

            string[] fullnames = new string[500];

            StreamReader sr = new StreamReader("../data/generator/fullnames.txt");

            for (int i = 0; i < 500; i++)
            {
                string row = sr.ReadLine();

                fullnames[i] = row;
            }

            ///////////////////////////////////////

            Random random = new Random();
        
            for (int i = 0; i < generatedCount; i++)
            {
                User newUser = new User();

                string generatedUsername = GenerateRandomUsername();

                if (userRep.UserExists(generatedUsername))
                {
                    i--;
                }

                newUser.username = generatedUsername;

                string password = 

                newUser.password = 
            }
        }

        private static string GenerateRandomUsername()
        {
            Random random = new Random();

            string username = "";

            for (int i = 0; i < 33; i++)
            {
                username += (char)random.Next(0, 257);
            }

            return username;
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

        private static string[] ParseDateIntervals(string dateIntervals)
        {
            string[] splittedDates = dateIntervals.Split("To");

            DateTime leftLimDate;
            DateTime rightLimDate;

            bool leftDateParsed = DateTime.TryParse(splittedDates[0], out leftLimDate);
            bool rightLimParsed = DateTime.TryParse(splittedDates[1], out rightLimDate);

            if (!leftDateParsed || !rightLimParsed)
            {
                throw new ArgumentException("Incorrect date format ('help' to help)");
            }

            int daysInterval = (leftLimDate - rightLimDate).Days;

            if (daysInterval <= 0)
            {
                throw new ArgumentException("Incorrect dates interval ('help' to help)");
            }

            return splittedDates;
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