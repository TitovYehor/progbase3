using System;
using Microsoft.Data.Sqlite;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using static System.Console;

using ProcessData;

namespace ConsoleApp
{
    public class ArgumentsProcessor
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
                        else if (command[0] == "export")
                        {
                            ProcessExport(command, postRep, commentRep);
                        }
                        else if (command[0] == "import")
                        {
                            ProcessImport(command, userRep, postRep, commentRep);
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

        static void ProcessExport(string[] command, PostRepository postRep, CommentRepository commentRep)
        {
            if (command.Length < 3)
            {
                throw new ArgumentException("Wrong command input. Command 'export' must have a form: export {zipName} {searchValue}");
            }

            string searchValue = String.Join(' ', command, 2, command.Length - 2);

            Post[] searchPosts = postRep.GetFiltredByTextPosts(searchValue);

            if (searchPosts.Length == 0)
            {
                WriteLine("Nothing found");
            }
            else
            {
                List<Comment> list = new List<Comment>();

                for (int i = 0; i < searchPosts.Length; i++)
                {
                    for (int j = 0; j < searchPosts[i].comments.Length; j++)
                    {
                        list.Add(searchPosts[i].comments[j]);
                    }
                }

                Comment[] comments = new Comment[list.Count];
                list.CopyTo(comments);

                if (ExportAndImportData.ExportPostsWithComments(command[1], searchPosts, comments))
                {
                    WriteLine("Data exported");
                }
                else
                {
                    WriteLine("Data not imported");
                }
            }
        }

        static void ProcessImport(string[] command, UserRepository userRep, PostRepository postRep, CommentRepository commentRep)
        {
            if (command.Length != 2)
            {
                throw new ArgumentException("Wrong command input. Command 'import' must have a form: import {zipName}");
            }

            if (ExportAndImportData.ImportPostsWithComments(command[1], userRep, postRep, commentRep))
            {
                WriteLine("Data imported");
            }
            else
            {
                WriteLine("Data not imported");
            }
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