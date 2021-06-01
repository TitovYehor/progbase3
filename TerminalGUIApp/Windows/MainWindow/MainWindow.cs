using Terminal.Gui;

using TerminalGUIApp.Windows.UserWindows;
using TerminalGUIApp.Windows.PostWindows;
using TerminalGUIApp.Windows.CommentWindows;
using TerminalGUIApp.Windows.ExportAndImportWindows;
using TerminalGUIApp.Windows.AuthenticationDialogs;

using ProcessData;

namespace TerminalGUIApp.Windows.MainWindow
{
    public class MainWindow : Window
    {
        private User currentUser;

        private UserRepository usersRepository; 
        private PostRepository postsRepository;
        private CommentRepository commentsRepository;

        private MenuBar mainMenu;
        private MenuBar helpMenu;

        private TextField currUserName;

        private Label usersLbl;
        private Button usersBtn;
        private Label postsLbl;
        private Button postsBtn;
        private Label commentsLbl;
        private Button commentsBtn;
        private Label exportLbl;
        private Button exportBtn;
        private Label importLbl;
        private Button importBtn;
        private Button registrationBtn;
        private Button loginBtn;
        private Button logOutBtn;
        private Button userContentBtn;


        public MainWindow()
        {
            mainMenu = new MenuBar(
                new MenuBarItem[] 
                {
                    new MenuBarItem ("_File", new MenuItem[]
                    {
                        new MenuItem("_Export...", "", OnExportOpen)
                        {
                            CanExecute = CanBeExecuted,
                        },
                        new MenuItem("_Import...", "", OnImportOpen)
                        {
                            CanExecute = CanBeExecuted,
                        },
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


            usersLbl = new Label("Users Window")
            {
                X = Pos.Center() - Pos.Percent(30),
                Y = Pos.Percent(33),
                Visible = false,
            };
            usersBtn = new Button("Open Users Window")
            {
                X = Pos.Left(usersLbl) - 5,
                Y = Pos.Top(usersLbl) + Pos.Percent(10),
                AutoSize = true,
                Visible = false,
            };
            usersBtn.Clicked += OnOpenUsers;
            this.Add(usersLbl, usersBtn);

            postsLbl = new Label("Posts Window")
            {
                X = Pos.Center(),
                Y = Pos.Top(usersLbl),
                Visible = false,
            };
            postsBtn = new Button("Open Posts Window")
            {
                X = Pos.Center(),
                Y = Pos.Top(usersBtn),
                AutoSize = true,
                Visible = false,
            };
            postsBtn.Clicked += OnOpenPosts;
            this.Add(postsLbl, postsBtn);

            commentsLbl = new Label("Comments Window")
            {
                X = Pos.Center() + Pos.Percent(30),
                Y = Pos.Top(usersLbl),
                Visible = false,
            };
            commentsBtn = new Button("Open Comments Window")
            {
                X = Pos.Left(commentsLbl) - 5,
                Y = Pos.Top(usersBtn),
                AutoSize = true,
                Visible = false,
            };
            commentsBtn.Clicked += OnOpenComments;
            this.Add(commentsLbl, commentsBtn);

            exportLbl = new Label("Export data:")
            {
                X = Pos.Left(postsBtn),
                Y = Pos.Bottom(postsBtn) + Pos.Percent(10),
                AutoSize = true,
                Visible = false,
            };
            exportBtn = new Button("Export")
            {
                X = Pos.Right(exportLbl) + Pos.Percent(1),
                Y = Pos.Top(exportLbl),
                AutoSize = true,
                Visible = false,
            };
            exportBtn.Clicked += OnExportOpen;
            this.Add(exportLbl, exportBtn);

            importLbl = new Label("Import data:")
            {
                X = Pos.Left(exportLbl),
                Y = Pos.Top(exportLbl) + Pos.Percent(5),
                AutoSize = true,
                Visible = false,
            };
            importBtn = new Button("Import")
            {
                X = Pos.Right(importLbl) + Pos.Percent(1),
                Y = Pos.Top(importLbl),
                AutoSize = true,
                Visible = false,
            };
            importBtn.Clicked += OnImportOpen;
            this.Add(importLbl, importBtn);

            userContentBtn = new Button($"Open all user content and data")
            {
                X = Pos.Center(),
                Y = Pos.Bottom(importBtn) + Pos.Percent(10),
                AutoSize = true,
                Visible = false,
            };
            userContentBtn.Clicked += OnOpenUserContent;
            this.Add(userContentBtn);

            Label currUserLbl = new Label("Logged user: ")
            {
                X = Pos.Percent(90),
                Y = Pos.Percent(3),
            };
            currUserName = new TextField("No one logged in")
            {
                X = Pos.Left(currUserLbl) - Pos.Percent(2),
                Y = Pos.Bottom(currUserLbl) + Pos.Percent(3),
                Width = Dim.Percent(10),
                TextAlignment = TextAlignment.Centered,
                ReadOnly = true,
            };
            this.Add(currUserLbl, currUserName);

            registrationBtn = new Button("Registration")
            {
                X = Pos.Left(currUserName) + Pos.Percent(1.1f),
                Y = Pos.Bottom(currUserName) + Pos.Percent(3),
                AutoSize = true,
            };
            registrationBtn.Clicked += OnRegister;
            loginBtn = new Button("Login")
            {
                X = Pos.Left(currUserLbl) + Pos.Percent(1),
                Y = Pos.Bottom(registrationBtn) + Pos.Percent(3),
                AutoSize = true,
            };
            loginBtn.Clicked += OnLogin;
            logOutBtn = new Button("Log out")
            {
                X = Pos.Left(loginBtn),
                Y = Pos.Top(loginBtn),
                AutoSize = true,
                Visible = false,
            };
            logOutBtn.Clicked += OnLogOut;
            this.Add(registrationBtn, loginBtn, logOutBtn);
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
        private bool CanBeExecuted()
        {
            if (currentUser == null)
            {
                return false;
            }
            
            return true;     
        }


        private void OnOpenUsers()
        {
            Toplevel top = Application.Top;

            MainUsersWindow win = new MainUsersWindow();
            win.SetCurrentUser(currentUser);
            win.SetRepository(usersRepository);

            RunWindow(win);

            UpdateUser();
        }
        private void OnOpenPosts()
        {
            Toplevel top = Application.Top;

            MainPostsWindow win = new MainPostsWindow();
            win.SetUser(currentUser);
            win.SetRepositories(postsRepository, commentsRepository);

            RunWindow(win);
        }
        private void OnOpenComments()
        {
            Toplevel top = Application.Top;

            MainCommentsWindow win = new MainCommentsWindow();
            win.SetUser(currentUser);
            win.SetRepository(commentsRepository);

            RunWindow(win);
        }
    

        private void UpdateUser()
        {
            currentUser = usersRepository.GetById(currentUser.id);

            currUserName.Text = currentUser.fullname;
        }

    
        private void OnImportOpen()
        {
            Toplevel top = Application.Top;

            ImportWindow win = new ImportWindow();
            win.SetRepositories(usersRepository, postsRepository, commentsRepository);

            RunWindow(win);
        }
        private void OnExportOpen()
        {
            Toplevel top = Application.Top;

            ExportWindow win = new ExportWindow();
            win.SetRepositories(postsRepository);

            RunWindow(win);
        }
    

        private void OnOpenUserContent()
        {
            Toplevel top = Application.Top;

            UserContentWindow win = new UserContentWindow();
            win.SetCurrentUser(currentUser);
            win.SetRepositories(usersRepository, postsRepository, commentsRepository);

            RunWindow(win);

            if (win.deleted)
            {
                OnLogOut();
            }
            else
            {
                UpdateUser();
            }
        }

    
        private void OnRegister()
        {
            RegisterDialog dialog = new RegisterDialog();
            dialog.SetRepository(usersRepository);

            Application.Run(dialog);
        }

        private void OnLogin()
        {
            LoginDialog dialog = new LoginDialog();
            dialog.SetRepository(usersRepository);

            Application.Run(dialog);

            if (dialog.logged)
            {
                currentUser = dialog.GetUser;   

                this.currUserName.Text = currentUser.fullname; 

                InterfaceOn();

                this.FocusNext();
            }
        }

        private void OnLogOut()
        {
            currentUser = null;

            InterfaceOff();
        }

        private void RunWindow(Window win)
        {
            Application.Top.Add(win);
            Application.Run(win);
            Application.Top.Add(this);
            this.SetFocus();
        }

        private void InterfaceOn()
        {
            this.currUserName.Text = currentUser.fullname;

            usersLbl.Visible = true;
            usersBtn.Visible = true;

            postsLbl.Visible = true;
            postsBtn.Visible = true;

            commentsLbl.Visible = true;
            commentsBtn.Visible = true;

            exportLbl.Visible = true;
            exportBtn.Visible = true;

            registrationBtn.Visible = false;

            importLbl.Visible = true;
            importBtn.Visible = true;

            loginBtn.Visible = false;
            logOutBtn.Visible = true;

            userContentBtn.Visible = true;

            Application.Refresh();
        }
        private void InterfaceOff()
        {
            this.currUserName.Text = "No one logged in";

            usersLbl.Visible = false;
            usersBtn.Visible = false;

            postsLbl.Visible = false;
            postsBtn.Visible = false;

            commentsLbl.Visible = false;
            commentsBtn.Visible = false;

            exportLbl.Visible = false;
            exportBtn.Visible = false;

            importLbl.Visible = false;
            importBtn.Visible = false;

            registrationBtn.Visible = true;

            loginBtn.Visible = true;
            logOutBtn.Visible = false;

            userContentBtn.Visible = false;

            Application.Refresh();
        }
    }
}