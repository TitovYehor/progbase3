using System.IO;
using Terminal.Gui;

using TerminalGUIApp.Windows.MainWindow;

using ProcessData;

namespace TerminalGUIApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string databasePath = "../data/database.db";

            if (!File.Exists(databasePath))
            {
                throw new FileNotFoundException("Database file not found. Program can not be launched");
            }

            UserRepository usersRepository = new UserRepository(databasePath);
            PostRepository postsRepository = new PostRepository(databasePath);
            CommentRepository commentsRepository = new CommentRepository(databasePath);

            Application.Init();
            Toplevel top = Application.Top;
    
            MainWindow win = new MainWindow();
            win.SetRepositories(usersRepository, postsRepository, commentsRepository);
            top.Add(win);
    
            Application.Run();
        }
    }
}
