using Terminal.Gui;

using TerminalGUIApp.Windows.UserWindows;

using ProcessData;

namespace TerminalGUIApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string databasePath = "../data/database.db";
            UserRepository usersRepository = new UserRepository(databasePath);
            PostRepository postsRepository = new PostRepository(databasePath);
            CommentRepository commentsRepository = new CommentRepository(databasePath);

            Application.Init();
            Toplevel top = Application.Top;
    
            MainUsersWindow win = new MainUsersWindow();
            win.SetRepository(usersRepository);
            top.Add(win);
    
            Application.Run();
        }
    }
}
