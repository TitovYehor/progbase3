using Terminal.Gui;

using ProcessData;

namespace TerminalGUIApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string databasePath = "../data/database.db";
            UserRepository usersRepository = new UserRepository(databasePath);

            Application.Init();
            Toplevel top = Application.Top;
    
            MainWindow win = new MainWindow();
            win.SetRepository(tasksRepository);
            top.Add(win);
    
            Application.Run();
        }
    }
}
