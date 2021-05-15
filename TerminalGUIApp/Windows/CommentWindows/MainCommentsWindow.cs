using System;
using System.Collections.Generic;
using Terminal.Gui;

using ProcessData;

namespace TerminalGUIApp.Windows.CommentWindows
{
    public class MainCommentsWindow : Window
    {
        private int pageSize = 10;
        private int page = 1;

        private string searchValue = "";
        private bool selecting = false;

        private ListView allCommentsListView;
        private CommentRepository commentsRepository;

        private MenuBar mainMenu;
        private MenuBar helpMenu;

        private Label pageLbl;
        private Label totalPagesLbl;
        private TextField searchInput;

        private FrameView frameView;

        private Button prevPageBtn;
        private Button nextPageBtn;

        private Button deleteCommentBtn;
        private Button editCommentBtn;

        private Label nullReferenceLbl = new Label();


        public MainCommentsWindow()
        {
            this.Title = "Comments Window";

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


            allCommentsListView = new ListView(new List<Comment>())
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            allCommentsListView.OpenSelectedItem += OnOpenComment;
            allCommentsListView.SelectedItemChanged += OnItemChanged;

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

            frameView = new FrameView("Comments")
            {
                X = Pos.Percent(15),
                Y = Pos.Percent(20),
                Width = Dim.Fill() - Dim.Percent(15),
                Height = pageSize + 2,
            };
            frameView.Add(allCommentsListView);
            this.Add(frameView);

            Button createNewCommentBtn = new Button("Create new comment")
            {
                X = Pos.Left(frameView) + Pos.Percent(10),
                Y = Pos.Bottom(frameView) + Pos.Percent(5),
            };
            createNewCommentBtn.Clicked += OnCreateButtonClick;
            deleteCommentBtn = new Button("Delete comment")
            {
                X = Pos.Right(createNewCommentBtn) + Pos.Percent(10),
                Y = Pos.Top(createNewCommentBtn),
            };
            deleteCommentBtn.Clicked += OnDeleteComment;
            editCommentBtn = new Button("Edit comment")
            {
                X = Pos.Right(deleteCommentBtn) + Pos.Percent(10),
                Y = Pos.Top(createNewCommentBtn),
            };
            editCommentBtn.Clicked += OnEditComment;
            this.Add(createNewCommentBtn, deleteCommentBtn, editCommentBtn);

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
        }



        public void SetRepository(CommentRepository commentsRepository)
        {
            this.commentsRepository = commentsRepository;

            UpdateCurrentPage();
        }

        private void UpdateCurrentPage()
        {
            int totalPages = commentsRepository.GetSearchPagesCount(pageSize, searchValue);

            if (page > totalPages)
            {
                page = 1;
            }

            this.pageLbl.Text = page.ToString();
            this.totalPagesLbl.Text = totalPages.ToString();

            if (!selecting)
            {
                this.allCommentsListView.SetSource(commentsRepository.GetSearchPage(searchValue, page, pageSize));

                if (allCommentsListView.Source.ToList().Count == 0)
                {
                    nullReferenceLbl = new Label("No records found")
                    {
                        X = Pos.Percent(45),
                        Y = Pos.Percent(50),
                    };
                    frameView.RemoveAll();
                    frameView.Add(nullReferenceLbl);

                    editCommentBtn.Visible = false;
                    deleteCommentBtn.Visible = false;
                }
                else
                {
                    frameView.RemoveAll();
                    frameView.Add(allCommentsListView);

                    editCommentBtn.Visible = true;
                    deleteCommentBtn.Visible = true;
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
            int totalPages = commentsRepository.GetSearchPagesCount(pageSize, searchValue);

            if (page >= totalPages)
            {
                return;
            }

            page += 1;
            
            UpdateCurrentPage();
        }

        private void OnPrevPage()
        {
            int totalPages = commentsRepository.GetSearchPagesCount(pageSize, searchValue);

            if (page == 1)
            {
                return;
            }

            page -= 1;
            
            UpdateCurrentPage();
        }


        private void OnDeleteComment()
        {
            int index = MessageBox.Query("Deleting comment", "Confirm deleting", "No", "Yes");

            if (index == 1)
            {
                int commentIndex = allCommentsListView.SelectedItem;
             
                if (commentIndex == -1 || commentIndex >= allCommentsListView.Source.ToList().Count)
                {
                    MessageBox.ErrorQuery("Deleting comment", "No comment selected", "Ok");

                    return; 
                }

                Comment selectedComment = (Comment)allCommentsListView.Source.ToList()[commentIndex];

                if (selectedComment == null)
                {
                    return;
                }
            
                if (!commentsRepository.DeleteById(selectedComment.id))
                {
                    MessageBox.ErrorQuery("Deleting comment", "Couldn't delete comment", "Ok");

                    return;
                }
                else
                {
                    int countOfPages = commentsRepository.GetSearchPagesCount(pageSize, searchValue);

                    if (page > countOfPages && page > 1)
                    {
                        page -= 1;
                    }

                    UpdateCurrentPage();
                }
            }
        }

        private void OnEditComment()
        {            
            int commentIndex = allCommentsListView.SelectedItem;
             
            if (commentIndex == -1 || commentIndex >= allCommentsListView.Source.ToList().Count)
            {
                MessageBox.ErrorQuery("Editing comment", "No comment selected", "Ok");

                return; 
            }

            Comment selectedComment = (Comment)allCommentsListView.Source.ToList()[commentIndex];

            EditCommentDialog dialog = new EditCommentDialog();
            dialog.SetComment(selectedComment);

            Application.Run(dialog);

            if (!dialog.canceled)
            {
                Comment changedComment = dialog.GetComment(); 

                if (changedComment == null)
                {
                    return;
                }

                changedComment.createdAt = selectedComment.createdAt;
            
                bool isUpdated = commentsRepository.Update(selectedComment.id, changedComment); 

                if (isUpdated)
                {
                    allCommentsListView.SetSource(commentsRepository.GetSearchPage(searchValue, page, pageSize));
                }
                else
                {
                    MessageBox.ErrorQuery("Editing comment", "Couldn't edit comment", "Ok");
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
            CreateCommentDialog dialog = new CreateCommentDialog();

            Application.Run(dialog);

            if (!dialog.canceled)
            {
                Comment comment = dialog.GetComment();

                if (comment == null)
                {
                    return;
                }

                comment.createdAt = DateTime.Now;
                int insertedId = commentsRepository.Insert(comment);
                comment.id = insertedId;

                allCommentsListView.SetSource(commentsRepository.GetSearchPage("", commentsRepository.GetSearchPagesCount(pageSize, ""), pageSize));

                allCommentsListView.SelectedItem = allCommentsListView.Source.ToList().Count - 1;

                allCommentsListView.OnOpenSelectedItem();

                UpdateCurrentPage();
            }
        }
    
    
        private void OnOpenComment(ListViewItemEventArgs args)
        {            
            Comment comment = (Comment)args.Value;

            OpenCommentDialog dialog = new OpenCommentDialog();
            dialog.SetComment(comment);

            Application.Run(dialog);

            if (dialog.deleted)
            {
                bool isDeleted = commentsRepository.DeleteById(comment.id); 

                if (isDeleted)
                {
                    int countOfPages = commentsRepository.GetSearchPagesCount(pageSize, searchValue);

                    if (page > countOfPages && page > 1)
                    {
                        page -= 1;
                    }

                    UpdateCurrentPage();
                }
                else
                {
                    MessageBox.ErrorQuery("Deleting comment", "Couldn't delete comment", "Ok");
                }
            }
        
            if (dialog.changed)
            {
                Comment changedComment = dialog.GetComment();

                changedComment.createdAt = comment.createdAt;

                bool isUpdated = commentsRepository.Update(comment.id, changedComment); 

                if (isUpdated)
                {
                    UpdateCurrentPage();
                }
                else
                {
                    MessageBox.ErrorQuery("Editing comment", "Couldn't edit comment", "Ok");
                }
            }
        }
    }
}