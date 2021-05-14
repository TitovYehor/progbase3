using System;
using System.Collections.Generic;
using Terminal.Gui;

using ProcessData;

namespace TerminalGUIApp.Windows.UserWindows
{
    public class MainUsersWindow : Window
    {
        private int pageSize = 10;
        private int page = 1;

        private string searchValue = "";
        private bool selecting = false;

        private ListView allUsersListView;
        private UserRepository usersRepository;

        private MenuBar mainMenu;
        private MenuBar helpMenu;

        private Label pageLbl;
        private Label totalPagesLbl;
        private TextField searchInput;

        private FrameView frameView;

        private Button prevPageBtn;
        private Button nextPageBtn;

        private Button deleteUserBtn;
        private Button editUserBtn;

        private Label nullReferenceLbl = new Label();


        public MainUsersWindow()
        {
            this.Title = "Users Window";

            mainMenu = new MenuBar(
                new MenuBarItem[] 
                {
                    new MenuBarItem ("_File", new MenuItem[]
                    {
                        new MenuItem("_New...", "", OnNew),
                        new MenuItem("_Quit", "", OnQuit)
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


            allUsersListView = new ListView(new List<User>())
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            allUsersListView.OpenSelectedItem += OnOpenUser;
            allUsersListView.SelectedItemChanged += OnItemChanged;

            prevPageBtn = new Button("Previous page")
            {
                X = Pos.Percent(35),
                Y = Pos.Percent(10),
            };
            pageLbl = new Label("?")
            {
                X = Pos.Right(prevPageBtn) + Pos.Percent(3),
                Y = Pos.Top(prevPageBtn),
                Width = 3,
            };
            Label separateLbl = new Label("of")
            {
                X = Pos.Right(pageLbl) + Pos.Percent(2),
                Y = Pos.Top(pageLbl),
            };
            totalPagesLbl = new Label("?")
            {
                X = Pos.Right(separateLbl) + Pos.Percent(3),
                Y = Pos.Top(pageLbl),
                Width = 3,
            };
            nextPageBtn = new Button("Next page")
            {
                X = Pos.Right(totalPagesLbl) + Pos.Percent(3),
                Y = Pos.Top(prevPageBtn),
            };
            nextPageBtn.Clicked += OnNextPage;
            prevPageBtn.Clicked += OnPrevPage;
            this.Add(prevPageBtn, pageLbl, separateLbl, totalPagesLbl, nextPageBtn);

            frameView = new FrameView("Users")
            {
                X = Pos.Percent(15),
                Y = Pos.Percent(20),
                Width = Dim.Fill() - Dim.Percent(15),
                Height = pageSize + 2,
            };
            frameView.Add(allUsersListView);
            this.Add(frameView);

            Button createNewUserBtn = new Button("Create new user")
            {
                X = Pos.Left(frameView) + Pos.Percent(10),
                Y = Pos.Bottom(frameView) + Pos.Percent(5),
            };
            createNewUserBtn.Clicked += OnCreateButtonClick;
            deleteUserBtn = new Button("Delete user")
            {
                X = Pos.Right(createNewUserBtn) + Pos.Percent(10),
                Y = Pos.Top(createNewUserBtn),
            };
            deleteUserBtn.Clicked += OnDeleteUser;
            editUserBtn = new Button("Edit user")
            {
                X = Pos.Right(deleteUserBtn) + Pos.Percent(10),
                Y = Pos.Top(createNewUserBtn),
            };
            editUserBtn.Clicked += OnEditUser;
            this.Add(createNewUserBtn, deleteUserBtn, editUserBtn);

            Label searchLbl = new Label("Seeking categories - ")
            {
                X = Pos.Percent(33),
                Y = Pos.Percent(15),
            };
            Label chooseSearchColumn = new Label("Username / Fullname - ")
            {
                X = Pos.Right(searchLbl),
                Y = Pos.Top(searchLbl),
                Width = Dim.Percent(10),
            };
            searchInput = new TextField()
            {
                X = Pos.Right(chooseSearchColumn) + Pos.Percent(1),
                Y = Pos.Top(searchLbl),
                Width = Dim.Percent(20),
            };
            searchInput.TextChanged += OnSearchChange;
            this.Add(searchLbl, chooseSearchColumn, searchInput);
        }



        public void SetRepository(UserRepository usersRepository)
        {
            this.usersRepository = usersRepository;

            UpdateCurrentPage();
        }

        private void UpdateCurrentPage()
        {
            int totalPages = usersRepository.GetSearchPagesCount(pageSize, searchValue);

            if (page > totalPages)
            {
                page = 1;
            }

            this.pageLbl.Text = page.ToString();
            this.totalPagesLbl.Text = totalPages.ToString();

            if (!selecting)
            {
                this.allUsersListView.SetSource(usersRepository.GetSearchPage(searchValue, page, pageSize));

                if (allUsersListView.Source.ToList().Count == 0)
                {
                    nullReferenceLbl = new Label("No records found")
                    {
                        X = Pos.Percent(45),
                        Y = Pos.Percent(50),
                    };
                    frameView.RemoveAll();
                    frameView.Add(nullReferenceLbl);

                    editUserBtn.Visible = false;
                    deleteUserBtn.Visible = false;
                }
                else
                {
                    frameView.RemoveAll();
                    frameView.Add(allUsersListView);

                    editUserBtn.Visible = true;
                    deleteUserBtn.Visible = true;
                }
            }
            else
            {
                selecting = false;
            }
            

            prevPageBtn.Visible = (page != 1);
            nextPageBtn.Visible = (page !< totalPages);
        }


        private void OnNew()
        {
            OnCreateButtonClick();
        }
        private void OnQuit()
        {
            Application.RequestStop();
        }
        private void OnAbout()
        {
            MessageBox.Query("About program", "Course work project. Made by a student of KP-01 Titov Egor, according to the lectures of the teacher Hadyniak Ruslan Anatoliiovych.", "Very interesting. Ok");
        }
        private void OnAllMenusClose()
        {
            MenuBar[] menus = new MenuBar[]{mainMenu, helpMenu};

            for (int i = 0; i < menus.Length; i++)
            {
                menus[i].CloseMenu();
            }
        }


        private void OnNextPage()
        {
            int totalPages = usersRepository.GetSearchPagesCount(pageSize, searchValue);

            if (page >= totalPages)
            {
                return;
            }

            page += 1;
            
            UpdateCurrentPage();
        }

        private void OnPrevPage()
        {
            int totalPages = usersRepository.GetSearchPagesCount(pageSize, searchValue);

            if (page == 1)
            {
                return;
            }

            page -= 1;
            
            UpdateCurrentPage();
        }


        private void OnDeleteUser()
        {
            int index = MessageBox.Query("Deleting user", "Confirm deleting", "No", "Yes");

            if (index == 1)
            {
                int userIndex = allUsersListView.SelectedItem;
             
                if (userIndex == -1 || userIndex >= allUsersListView.Source.ToList().Count)
                {
                    MessageBox.ErrorQuery("Deleting user", "No user selected", "Ok");

                    return; 
                }

                User selectedUser = (User)allUsersListView.Source.ToList()[userIndex];

                if (selectedUser == null)
                {
                    return;
                }
            
                if (!usersRepository.DeleteByUsername(selectedUser.username))
                {
                    MessageBox.ErrorQuery("Deleting user", "Couldn't delete user", "Ok");

                    return;
                }
                else
                {
                    int countOfPages = usersRepository.GetSearchPagesCount(pageSize, searchValue);

                    if (page > countOfPages && page > 1)
                    {
                        page -= 1;
                    }

                    UpdateCurrentPage();
                }
            }
        }

        private void OnEditUser()
        {            
            int userIndex = allUsersListView.SelectedItem;
             
            if (userIndex == -1 || userIndex >= allUsersListView.Source.ToList().Count)
            {
                MessageBox.ErrorQuery("Editing user", "No user selected", "Ok");

                return; 
            }

            User selectedUser = (User)allUsersListView.Source.ToList()[userIndex];

            EditUserDialog dialog = new EditUserDialog();
            dialog.SetUser(selectedUser);

            Application.Run(dialog);

            if (!dialog.canceled)
            {
                User changedUser = dialog.GetUser(); 

                if (changedUser == null)
                {
                    return;
                }

                changedUser.createdAt = selectedUser.createdAt;
            
                bool isUpdated = usersRepository.Update(selectedUser.id, changedUser); 

                if (isUpdated)
                {
                    allUsersListView.SetSource(usersRepository.GetSearchPage(searchValue, page, pageSize));
                }
                else
                {
                    MessageBox.ErrorQuery("Editing user", "Couldn't edit user", "Ok");
                }
            }
        }


        private void OnSearchChange(NStack.ustring text)
        {
            searchValue = searchInput.Text.ToString();
            
            UpdateCurrentPage();
        }


        private void OnItemChanged(ListViewItemEventArgs args)
        {
            selecting = true;

            UpdateCurrentPage();
        }


        private void OnCreateButtonClick()
        {
            CreateUserDialog dialog = new CreateUserDialog();

            Application.Run(dialog);

            if (!dialog.canceled)
            {
                User user = dialog.GetUser();

                if (user == null)
                {
                    return;
                }

                user.createdAt = DateTime.Now;
                int insertedId = usersRepository.Insert(user);
                user.id = insertedId;

                allUsersListView.SetSource(usersRepository.GetSearchPage("", usersRepository.GetSearchPagesCount(pageSize, ""), pageSize));

                allUsersListView.SelectedItem = allUsersListView.Source.ToList().Count - 1;

                allUsersListView.OnOpenSelectedItem();

                UpdateCurrentPage();
            }
        }
    
    
        private void OnOpenUser(ListViewItemEventArgs args)
        {            
            User user = (User)args.Value;

            OpenUserDialog dialog = new OpenUserDialog();
            dialog.SetUser(user);

            Application.Run(dialog);

            if (dialog.deleted)
            {
                bool isDeleted = usersRepository.DeleteByUsername(user.username); 

                if (isDeleted)
                {
                    int countOfPages = usersRepository.GetSearchPagesCount(pageSize, searchValue);

                    if (page > countOfPages && page > 1)
                    {
                        page -= 1;
                    }

                    UpdateCurrentPage();
                }
                else
                {
                    MessageBox.ErrorQuery("Deleting user", "Couldn't delete user", "Ok");
                }
            }
        
            if (dialog.changed)
            {
                User changedUser = dialog.GetUser();

                changedUser.createdAt = user.createdAt;

                bool isUpdated = usersRepository.Update(user.id, changedUser); 

                if (isUpdated)
                {
                    UpdateCurrentPage();
                }
                else
                {
                    MessageBox.ErrorQuery("Editing user", "Couldn't edit user", "Ok");
                }
            }
        }
    }
}