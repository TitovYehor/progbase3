using System;
using System.Collections.Generic;
using Terminal.Gui;

using TerminalGUIApp.Windows.ExportAndImportWindows;

using ProcessData;

namespace TerminalGUIApp.Windows.PostWindows
{
    public class MainPostsWindow : Window
    {
        private int pageSize = 10;
        private int page = 1;

        private string searchValue = "";
        private bool selecting = false;

        private ListView allPostsListView;
        private PostRepository postsRepository;

        private MenuBar mainMenu;
        private MenuBar helpMenu;

        private Label pageLbl;
        private Label totalPagesLbl;
        private TextField searchInput;

        private FrameView frameView;

        private Button prevPageBtn;
        private Button nextPageBtn;

        private Button deletePostBtn;
        private Button editPostBtn;

        private Label nullReferenceLbl = new Label();


        public MainPostsWindow()
        {
            this.Title = "Posts Window";

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


            allPostsListView = new ListView(new List<Post>())
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            allPostsListView.OpenSelectedItem += OnOpenPost;
            allPostsListView.SelectedItemChanged += OnItemChanged;

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

            frameView = new FrameView("Posts")
            {
                X = Pos.Percent(15),
                Y = Pos.Percent(20),
                Width = Dim.Fill() - Dim.Percent(15),
                Height = pageSize + 2,
            };
            frameView.Add(allPostsListView);
            this.Add(frameView);

            Button createNewPostBtn = new Button("Create new post")
            {
                X = Pos.Left(frameView) + Pos.Percent(10),
                Y = Pos.Bottom(frameView) + Pos.Percent(5),
            };
            createNewPostBtn.Clicked += OnCreateButtonClick;
            deletePostBtn = new Button("Delete post")
            {
                X = Pos.Right(createNewPostBtn) + Pos.Percent(10),
                Y = Pos.Top(createNewPostBtn),
            };
            deletePostBtn.Clicked += OnDeletePost;
            editPostBtn = new Button("Edit post")
            {
                X = Pos.Right(deletePostBtn) + Pos.Percent(10),
                Y = Pos.Top(createNewPostBtn),
            };
            editPostBtn.Clicked += OnEditPost;
            this.Add(createNewPostBtn, deletePostBtn, editPostBtn);

            Label searchLbl = new Label("Seeking categories - ")
            {
                X = Pos.Percent(33),
                Y = Pos.Percent(15),
            };
            Label chooseSearchColumn = new Label("Content - ")
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

            // Label exportLbl = new Label("Export data:")
            // {
            //     X = Pos.Percent(45),
            //     Y = Pos.Top(createNewPostBtn) + Pos.Percent(10),
            //     AutoSize = true,
            // };
            // Button exportBtn = new Button("Export")
            // {
            //     X = Pos.Left(exportLbl) + Pos.Percent(5),
            //     Y = Pos.Top(exportLbl),
            //     AutoSize = true,
            // };
            // exportBtn.Clicked += OnExportOpen;
            // this.Add(exportLbl, exportBtn);

            // Label importLbl = new Label("Import data:")
            // {
            //     X = Pos.Percent(45),
            //     Y = Pos.Top(exportLbl) + Pos.Percent(5),
            //     AutoSize = true,
            // };
            // Button importBtn = new Button("Import")
            // {
            //     X = Pos.Left(importLbl) + Pos.Percent(5),
            //     Y = Pos.Top(importLbl),
            //     AutoSize = true,
            // };
            // importBtn.Clicked += OnImportOpen;
            // this.Add(importLbl, importBtn);
        }



        public void SetRepository(PostRepository postsRepository)
        {
            this.postsRepository = postsRepository;

            UpdateCurrentPage();
        }

        private void UpdateCurrentPage()
        {
            int totalPages = postsRepository.GetSearchPagesCount(pageSize, searchValue);

            if (page > totalPages)
            {
                page = 1;
            }

            this.pageLbl.Text = page.ToString();
            this.totalPagesLbl.Text = totalPages.ToString();

            if (!selecting)
            {
                this.allPostsListView.SetSource(postsRepository.GetSearchPage(searchValue, page, pageSize));

                if (allPostsListView.Source.ToList().Count == 0)
                {
                    nullReferenceLbl = new Label("No records found")
                    {
                        X = Pos.Percent(45),
                        Y = Pos.Percent(50),
                    };
                    frameView.RemoveAll();
                    frameView.Add(nullReferenceLbl);

                    editPostBtn.Visible = false;
                    deletePostBtn.Visible = false;
                }
                else
                {
                    frameView.RemoveAll();
                    frameView.Add(allPostsListView);

                    editPostBtn.Visible = true;
                    deletePostBtn.Visible = true;
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
            int totalPages = postsRepository.GetSearchPagesCount(pageSize, searchValue);

            if (page >= totalPages)
            {
                return;
            }

            page += 1;
            
            UpdateCurrentPage();
        }

        private void OnPrevPage()
        {
            int totalPages = postsRepository.GetSearchPagesCount(pageSize, searchValue);

            if (page == 1)
            {
                return;
            }

            page -= 1;
            
            UpdateCurrentPage();
        }


        private void OnDeletePost()
        {
            int index = MessageBox.Query("Deleting post", "Confirm deleting", "No", "Yes");

            if (index == 1)
            {
                int postIndex = allPostsListView.SelectedItem;
             
                if (postIndex == -1 || postIndex >= allPostsListView.Source.ToList().Count)
                {
                    MessageBox.ErrorQuery("Deleting post", "No post selected", "Ok");

                    return; 
                }

                Post selectedPost = (Post)allPostsListView.Source.ToList()[postIndex];

                if (selectedPost == null)
                {
                    return;
                }
            
                if (!postsRepository.DeleteById(selectedPost.id))
                {
                    MessageBox.ErrorQuery("Deleting post", "Couldn't delete post", "Ok");

                    return;
                }
                else
                {
                    int countOfPages = postsRepository.GetSearchPagesCount(pageSize, searchValue);

                    if (page > countOfPages && page > 1)
                    {
                        page -= 1;
                    }

                    UpdateCurrentPage();
                }
            }
        }

        private void OnEditPost()
        {            
            int postIndex = allPostsListView.SelectedItem;
             
            if (postIndex == -1 || postIndex >= allPostsListView.Source.ToList().Count)
            {
                MessageBox.ErrorQuery("Editing post", "No post selected", "Ok");

                return; 
            }

            Post selectedPost = (Post)allPostsListView.Source.ToList()[postIndex];

            EditPostDialog dialog = new EditPostDialog();
            dialog.SetPost(selectedPost);

            Application.Run(dialog);

            if (!dialog.canceled)
            {
                Post changedPost = dialog.GetPost(); 

                if (changedPost == null)
                {
                    return;
                }

                changedPost.createdAt = selectedPost.createdAt;
            
                bool isUpdated = postsRepository.Update(selectedPost.id, changedPost); 

                if (isUpdated)
                {
                    allPostsListView.SetSource(postsRepository.GetSearchPage(searchValue, page, pageSize));
                }
                else
                {
                    MessageBox.ErrorQuery("Editing post", "Couldn't edit post", "Ok");
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
            CreatePostDialog dialog = new CreatePostDialog();

            Application.Run(dialog);

            if (!dialog.canceled)
            {
                Post post = dialog.GetPost();

                if (post == null)
                {
                    return;
                }

                post.createdAt = DateTime.Now;
                int insertedId = postsRepository.Insert(post);
                post.id = insertedId;

                allPostsListView.SetSource(postsRepository.GetSearchPage("", postsRepository.GetSearchPagesCount(pageSize, ""), pageSize));

                allPostsListView.SelectedItem = allPostsListView.Source.ToList().Count - 1;

                allPostsListView.OnOpenSelectedItem();

                UpdateCurrentPage();
            }
        }
    
    
        private void OnOpenPost(ListViewItemEventArgs args)
        {            
            Post post = (Post)args.Value;

            OpenPostDialog dialog = new OpenPostDialog();
            dialog.SetPost(post);

            Application.Run(dialog);

            if (dialog.deleted)
            {
                bool isDeleted = postsRepository.DeleteById(post.id); 

                if (isDeleted)
                {
                    int countOfPages = postsRepository.GetSearchPagesCount(pageSize, searchValue);

                    if (page > countOfPages && page > 1)
                    {
                        page -= 1;
                    }

                    UpdateCurrentPage();
                }
                else
                {
                    MessageBox.ErrorQuery("Deleting post", "Couldn't delete post", "Ok");
                }
            }
        
            if (dialog.changed)
            {
                Post changedPost = dialog.GetPost();

                changedPost.createdAt = post.createdAt;

                bool isUpdated = postsRepository.Update(post.id, changedPost); 

                if (isUpdated)
                {
                    UpdateCurrentPage();
                }
                else
                {
                    MessageBox.ErrorQuery("Editing post", "Couldn't edit post", "Ok");
                }
            }
        }
    
        // private void OnImportOpen()
        // {
        //     ImportWindow win = new ImportWindow();

        //     Application.Run(win);
        // }

        // private void OnExportOpen()
        // {
        //     ExportWindow win = new ExportWindow();

        //     Application.Run(win);
        // }
    }
}