using System;
using System.Collections.Generic;
using Terminal.Gui;

using ProcessData;

namespace TerminalGUIApp.Windows.UserWindows
{
    public class MainUsersWindow : Window
    {
        private User currUser;
        bool canEdit = false;

        private int pageSize = 10;
        private int page = 1;

        private string searchValue = "";
        private bool selecting = false;

        private ListView allUsersListView;
        private ListView currUserListView;
        private UserRepository usersRepository;

        private MenuBar mainMenu;

        private Label pageLbl;
        private Label totalPagesLbl;
        private TextField searchInput;

        private FrameView frameView;
        private FrameView currUserFrameView;

        private Button prevPageBtn;
        private Button nextPageBtn;
        private Button firstPageBtn;
        private Button lastPageBtn;

        private Button createNewUserBtn;
        private Button deleteUserBtn;
        private Button editUserBtn;

        private Label nullReferenceLbl = new Label();


        public MainUsersWindow()
        {
            this.Title = "Users Window";

            mainMenu = new MenuBar(
                new MenuBarItem[] 
                {
                    new MenuBarItem ("_Users", new MenuItem[]
                    {
                        new MenuItem("_New...", "", OnNew),
                        new MenuItem("_Quit", "", OnQuit)
                    })
                })
            {
                Width = Dim.Percent(5),
            };
            this.Add(mainMenu);


            allUsersListView = new ListView(new List<User>())
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            allUsersListView.OpenSelectedItem += OnOpenUser;
            allUsersListView.SelectedItemChanged += OnItemChanged;

            currUserListView = new ListView(new List<User>())
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            currUserListView.OpenSelectedItem += OnOpenUser;

            firstPageBtn = new Button("First page")
            {
                X = Pos.Percent(27),
                Y = Pos.Percent(10),
                Visible = false,
            };
            prevPageBtn = new Button("Previous page")
            {
                X = Pos.Right(firstPageBtn) + Pos.Percent(3),
                Y = Pos.Top(firstPageBtn),
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
            lastPageBtn = new Button("Last page")
            {
                X = Pos.Right(nextPageBtn) + Pos.Percent(3),
                Y = Pos.Top(nextPageBtn),
            };
            firstPageBtn.Clicked += OnFirstPage;
            nextPageBtn.Clicked += OnNextPage;
            prevPageBtn.Clicked += OnPrevPage;
            lastPageBtn.Clicked += OnLastPage;
            this.Add(firstPageBtn, prevPageBtn, pageLbl, separateLbl, totalPagesLbl, nextPageBtn, lastPageBtn);

            frameView = new FrameView("Users")
            {
                X = Pos.Percent(15),
                Y = Pos.Percent(20),
                Width = Dim.Fill() - Dim.Percent(15),
                Height = pageSize + 2,
            };
            frameView.Add(allUsersListView);
            this.Add(frameView);

            createNewUserBtn = new Button("Create new user")
            {
                X = Pos.Left(frameView) + Pos.Percent(10),
                Y = Pos.Bottom(frameView) + Pos.Percent(5),
                Visible = false,
            };
            createNewUserBtn.Clicked += OnCreateButtonClick;
            deleteUserBtn = new Button("Delete user")
            {
                X = Pos.Right(createNewUserBtn) + Pos.Percent(10),
                Y = Pos.Top(createNewUserBtn),
                Visible = false,
            };
            deleteUserBtn.Clicked += OnDeleteUser;
            editUserBtn = new Button("Edit user")
            {
                X = Pos.Right(deleteUserBtn) + Pos.Percent(10),
                Y = Pos.Top(createNewUserBtn),
                Visible = false,
            };
            editUserBtn.Clicked += OnEditUser;
            this.Add(createNewUserBtn, deleteUserBtn, editUserBtn);

            currUserFrameView = new FrameView("Current user")
            {
                X = Pos.Left(frameView),
                Y = Pos.Bottom(createNewUserBtn) + Pos.Percent(5),
                Width = Dim.Width(frameView),
                Height = 3,
            };
            currUserFrameView.Add(currUserListView);
            this.Add(currUserFrameView);
            
            Label searchLbl = new Label("Seeking categories - ")
            {
                X = Pos.Percent(33),
                Y = Pos.Percent(15),
            };
            Label chooseSearchColumn = new Label("Username / Fullname - ")
            {
                X = Pos.Right(searchLbl),
                Y = Pos.Top(searchLbl),
            };
            searchInput = new TextField()
            {
                X = Pos.Right(chooseSearchColumn) + Pos.Percent(1),
                Y = Pos.Top(searchLbl),
                Width = Dim.Percent(20),
            };
            searchInput.TextChanged += OnSearchChange;
            this.Add(searchLbl, chooseSearchColumn, searchInput);

            Application.Refresh();
        }



        public void SetRepository(UserRepository usersRepository)
        {
            this.usersRepository = usersRepository;

            UpdateCurrentPage();
        }

        public void SetCurrentUser(User user)
        {
            this.currUser = user;

            this.currUserListView.SetSource(new List<User>(){currUser});
            this.currUserFrameView.Add(this.currUserListView);
          
            if (currUser.role == "admin")
            {
                this.createNewUserBtn.Visible = true;
                this.editUserBtn.Visible = true;
                this.deleteUserBtn.Visible = true;

                canEdit = true;
            }
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

            editUserBtn.Visible = canEdit;
            deleteUserBtn.Visible = canEdit;

            prevPageBtn.Visible = (page != 1);
            nextPageBtn.Visible = (page !< totalPages);
            firstPageBtn.Visible = prevPageBtn.Visible;
            lastPageBtn.Visible = nextPageBtn.Visible;
        }


        private void OnNew()
        {
            OnCreateButtonClick();
        }
        private void OnQuit()
        {
            Application.RequestStop();
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

        private void OnLastPage()
        {
            int totalPages = usersRepository.GetSearchPagesCount(pageSize, searchValue);

            page = totalPages;

            UpdateCurrentPage();
        }

        private void OnFirstPage()
        {
            page = 1;

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

            if (dialog.accepted)
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

            User changedUser = (User)args.Value; 

            if (changedUser.id == currUser.id || currUser.role == "admin")
            {
                canEdit = true;
            }
            else
            {
                canEdit = false;
            }

            UpdateCurrentPage();
        }


        private void OnCreateButtonClick()
        {
            CreateUserDialog dialog = new CreateUserDialog();

            Application.Run(dialog);

            if (dialog.accepted)
            {
                User user = dialog.GetUser();

                if (user == null)
                {
                    return;
                }

                Authentication authentication = new Authentication(usersRepository);

                if (!authentication.Register(user))
                {
                    MessageBox.ErrorQuery("Creating user", "Can not create user. User with that username already exists.", "Ok");
                }
                else
                {
                    allUsersListView.SetSource(usersRepository.GetSearchPage("", usersRepository.GetSearchPagesCount(pageSize, ""), pageSize));

                    allUsersListView.SelectedItem = allUsersListView.Source.ToList().Count - 1;

                    allUsersListView.OnOpenSelectedItem();

                    UpdateCurrentPage();
                }
            }
        }
    
    
        private void OnOpenUser(ListViewItemEventArgs args)
        {            
            User user = (User)args.Value;

            OpenUserDialog dialog = new OpenUserDialog();
            dialog.canEdit = canEdit;
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