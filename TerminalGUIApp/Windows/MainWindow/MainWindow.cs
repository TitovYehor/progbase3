using Terminal.Gui;

using TerminalGUIApp.Windows.UserWindows;
using TerminalGUIApp.Windows.PostWindows;
using TerminalGUIApp.Windows.CommentWindows;
using TerminalGUIApp.Windows.ExportAndImportWindows;

using ProcessData;

namespace TerminalGUIApp.Windows.MainWindow
{
    public class MainWindow : Window
    {
        private UserRepository usersRepository; 
        private PostRepository postsRepository;
        private CommentRepository commentsRepository;

        private MenuBar mainMenu;
        private MenuBar helpMenu;

    
        public MainWindow()
        {
            mainMenu = new MenuBar(
                new MenuBarItem[] 
                {
                    new MenuBarItem ("_File", new MenuItem[]
                    {
                        new MenuItem("_Export...", "", OnExportOpen),
                        new MenuItem("_Import...", "", OnImportOpen),
                        new MenuItem("_Quit", "", OnQuit),
                    })
                })
            {
                Width = Dim.Percent(5),
            };
            mainMenu.MenuOpening += OnAllMenusClose;
            helpMenu = new MenuBar(
                new MenuBarItem[]
                {
                    new MenuBarItem("_Help", new MenuItem[]
                    {
                        new MenuItem("_About", "", OnAbout)
                    })
                }
            )
            {
                X = Pos.Left(mainMenu),
                Y = Pos.Bottom(mainMenu) + Pos.Percent(1),
                Width = Dim.Percent(5)
            };
            this.Add(mainMenu, helpMenu);


            Label usersLbl = new Label("Users Window")
            {
                X = Pos.Percent(25),
                Y = Pos.Percent(33),
            };
            Button usersBtn = new Button("Open Users Window")
            {
                X = Pos.Left(usersLbl) - 5,
                Y = Pos.Top(usersLbl) + Pos.Percent(10),
                AutoSize = true,
            };
            usersBtn.Clicked += OnOpenUsers;
            this.Add(usersLbl, usersBtn);

            Label postsLbl = new Label("Posts Window")
            {
                X = Pos.Percent(50),
                Y = Pos.Top(usersLbl),
            };
            Button postsBtn = new Button("Open Posts Window")
            {
                X = Pos.Left(postsLbl) - 5,
                Y = Pos.Top(usersBtn),
                AutoSize = true,
            };
            postsBtn.Clicked += OnOpenPosts;
            this.Add(postsLbl, postsBtn);

            Label commentsLbl = new Label("Comments Window")
            {
                X = Pos.Percent(75),
                Y = Pos.Top(usersLbl),
            };
            Button commentsBtn = new Button("Open Comments Window")
            {
                X = Pos.Left(commentsLbl) - 5,
                Y = Pos.Top(usersBtn),
                AutoSize = true,
            };
            commentsBtn.Clicked += OnOpenComments;
            this.Add(commentsLbl, commentsBtn);

            Label exportLbl = new Label("Export data:")
            {
                X = Pos.Percent(45),
                Y = Pos.Bottom(postsBtn) + Pos.Percent(10),
                AutoSize = true,
            };
            Button exportBtn = new Button("Export")
            {
                X = Pos.Right(exportLbl) + Pos.Percent(1),
                Y = Pos.Top(exportLbl),
                AutoSize = true,
            };
            exportBtn.Clicked += OnExportOpen;
            this.Add(exportLbl, exportBtn);

            Label importLbl = new Label("Import data:")
            {
                X = Pos.Left(exportLbl),
                Y = Pos.Top(exportLbl) + Pos.Percent(5),
                AutoSize = true,
            };
            Button importBtn = new Button("Import")
            {
                X = Pos.Right(importLbl) + Pos.Percent(1),
                Y = Pos.Top(importLbl),
                AutoSize = true,
            };
            importBtn.Clicked += OnImportOpen;
            this.Add(importLbl, importBtn);
        }

        public void SetRepositories(UserRepository usersRepository, PostRepository postsRepository, CommentRepository commentsRepository)
        {
            this.usersRepository = usersRepository;
            this.postsRepository = postsRepository;
            this.commentsRepository = commentsRepository;
        }


        private void OnQuit()
        {
            Application.RequestStop();
        }
        private void OnAbout()
        {
            MessageBox.Query("About program", "Course work project. Made by a student of KP-01 Titov Egor, according to the lectures of the teacher Hadyniak Ruslan Anatoliiovych.", "Very informative. Ok");
        }
        private void OnAllMenusClose()
        {
            MenuBar[] menus = new MenuBar[]{mainMenu, helpMenu};

            for (int i = 0; i < menus.Length; i++)
            {
                menus[i].CloseMenu();
            }
        }
    
        private void OnOpenUsers()
        {
            MainUsersWindow win = new MainUsersWindow();
            win.SetRepository(usersRepository);

            Application.Run(win);
        }

        private void OnOpenPosts()
        {
            MainPostsWindow win = new MainPostsWindow();
            win.SetRepository(postsRepository);

            Application.Run(win);
        }

        private void OnOpenComments()
        {
            MainCommentsWindow win = new MainCommentsWindow();
            win.SetRepository(commentsRepository);

            Application.Run(win);
        }
    
    
        private void OnImportOpen()
        {
            ImportWindow win = new ImportWindow();
            win.SetRepositories(usersRepository, postsRepository, commentsRepository);

            Application.Run(win);
        }

        private void OnExportOpen()
        {
            ExportWindow win = new ExportWindow();
            win.SetRepositories(postsRepository);

            Application.Run(win);
        }
    }
}