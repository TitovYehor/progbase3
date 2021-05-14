using Terminal.Gui;

using TerminalGUIApp.Windows.UserWindows;
using TerminalGUIApp.Windows.PostWindows;
//using TerminalGUIApp.Windows.CommentWindows;

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
    
            MainPostsWindow win = new MainPostsWindow();
            win.SetRepository(postsRepository);
            top.Add(win);
    
            Application.Run();
        }
    }
}
