using System;
using System.Collections.Generic;
using Terminal.Gui;

using ProcessData;
using TerminalGUIApp.Windows.CommentWindows;

namespace TerminalGUIApp.Windows.PostWindows
{
    public class MainPostsWindow : Window
    {
        private User currentUser;
        private Post currentPost;
        private Comment currentComment;

        private int pageSize = 10;
        private int page = 1;

        private string searchValue = "";
        private bool selecting = false;

        private ListView allPostsListView;
        private PostRepository postsRepository;

        private MenuBar mainMenu;

        private Label pageLbl;
        private Label totalPagesLbl;
        private TextField searchInput;

        private FrameView frameView;

        private Button prevPageBtn;
        private Button nextPageBtn;
        private Button firstPageBtn;
        private Button lastPageBtn;

        private Button deletePostBtn;
        private Button editPostBtn;

        //private Label nullReferenceLbl = new Label();

        // for comments
        private ListView allCommentsListView;
        private ListView pinCommentListView;
        private FrameView commentsFrameView;
        private FrameView pinnedCommentFrameView;
        private CommentRepository commentsRepository;

        private Button createCommentBtn;
        private Button deleteCommentBtn;
        private Button editCommentBtn;

        //private Label nullCommentReferenceLbl = new Label();



        public MainPostsWindow()
        {
            this.Title = "Posts Window";

            mainMenu = new MenuBar(
                new MenuBarItem[] 
                {
                    new MenuBarItem ("_Posts", new MenuItem[]
                    {
                        new MenuItem("_New...", "", OnNew),
                        new MenuItem("_Quit", "", OnQuit)
                    })
                })
            {
                Width = Dim.Percent(5),
            };
            this.Add(mainMenu);


            allPostsListView = new ListView(new List<Post>())
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            allPostsListView.OpenSelectedItem += OnOpenPost;
            allPostsListView.SelectedItemChanged += OnPostItemChanged;

            allCommentsListView = new ListView(new List<Comment>())
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            allCommentsListView.OpenSelectedItem += OnOpenComment;
            allCommentsListView.SelectedItemChanged += OnCommentItemChanged;

            pinCommentListView = new ListView(new List<Comment>())
            {       
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            pinCommentListView.OpenSelectedItem += OnOpenComment;

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
                Visible = false,
            };
            deletePostBtn.Clicked += OnDeletePost;
            editPostBtn = new Button("Edit post")
            {
                X = Pos.Right(deletePostBtn) + Pos.Percent(10),
                Y = Pos.Top(createNewPostBtn),
                Visible = false,
            };
            editPostBtn.Clicked += OnEditPost;
            this.Add(createNewPostBtn, deletePostBtn, editPostBtn);

            createCommentBtn = new Button("Comment post")
            {
                X = Pos.Left(createNewPostBtn),
                Y = Pos.Bottom(createNewPostBtn) + Pos.Percent(5),
                AutoSize = true,
                Visible = false,
            };
            createCommentBtn.Clicked += OnCreateCommentBtnClick;
            this.Add(createCommentBtn);

            commentsFrameView = new FrameView("Comments of post")
            {
                X = Pos.Left(frameView),
                Y = Pos.Bottom(createCommentBtn) + Pos.Percent(5),
                Width = Dim.Width(frameView),
                Height = Dim.Height(frameView),
            };
            commentsFrameView.Add(allCommentsListView);
            this.Add(commentsFrameView);

            deleteCommentBtn = new Button("Delete comment")
            {
                X = Pos.Left(commentsFrameView),
                Y = Pos.Bottom(commentsFrameView) + Pos.Percent(5),
            };
            deleteCommentBtn.Clicked += OnDeleteComment;
            editCommentBtn = new Button("Edit comment")
            {
                X = Pos.Right(deleteCommentBtn) + Pos.Percent(5),
                Y = Pos.Top(deleteCommentBtn),
            };
            editCommentBtn.Clicked += OnEditComment;
            this.Add(deleteCommentBtn, editCommentBtn);
        }



        public void SetRepositories(PostRepository postsRepository, CommentRepository commentsRepository)
        {
            this.postsRepository = postsRepository;

            this.commentsRepository = commentsRepository;

            UpdateCurrentPage();
        }

        public void SetUser(User user)
        {
            this.currentUser = user;
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

                UpdateComments();

                if (allPostsListView.Source.ToList().Count == 0)
                {
                    Label nullReferenceLbl = new Label("No posts found")
                    {
                        X = Pos.Percent(45),
                        Y = Pos.Percent(50),
                    };
                    frameView.RemoveAll();
                    frameView.Add(nullReferenceLbl);

                    Label nullCommentReferenceLbl = new Label("No comments found")
                    {
                        X = Pos.Percent(45),
                        Y = Pos.Percent(50),
                    };
                    commentsFrameView.RemoveAll();
                    commentsFrameView.Add(nullCommentReferenceLbl);

                    editPostBtn.Visible = false;
                    deletePostBtn.Visible = false;
                }
                else
                {
                    frameView.RemoveAll();
                    frameView.Add(allPostsListView);

                    //UpdateComments();

                    SetPostRoleLimits();
                }
            }
            else
            {
                selecting = false;

                UpdateComments();
            }

            prevPageBtn.Visible = (page != 1);
            nextPageBtn.Visible = (page !< totalPages);
            firstPageBtn.Visible = prevPageBtn.Visible;
            lastPageBtn.Visible = nextPageBtn.Visible;
        }

        private void UpdateComments()
        {
            if (allPostsListView.Source.ToList().Count != 0)
            {
                this.currentPost = (Post)allPostsListView.Source.ToList()[allPostsListView.SelectedItem];

                this.allCommentsListView.SetSource(commentsRepository.GetByPostId(currentPost.id));

                if (allCommentsListView.Source.ToList().Count == 0)
                {
                    Label nullReferenceLbl = new Label("No comments found")
                    {
                        X = Pos.Percent(45),
                        Y = Pos.Percent(50),
                    };
                    commentsFrameView.RemoveAll();
                    commentsFrameView.Add(nullReferenceLbl);
                }
                else
                {
                    this.currentComment = (Comment)allCommentsListView.Source.ToList()[allCommentsListView.SelectedItem];

                    commentsFrameView.RemoveAll();
                    commentsFrameView.Add(allCommentsListView);
                }
            }
            
            createCommentBtn.Visible = (allPostsListView.Source.ToList().Count != 0);

            SetCommentRoleLimits();
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
        private void OnLastPage()
        {
            int totalPages = postsRepository.GetSearchPagesCount(pageSize, searchValue);

            page = totalPages;

            UpdateCurrentPage();
        }
        private void OnFirstPage()
        {
            page = 1;

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

            if (dialog.accepted)
            {
                Post changedPost = dialog.GetPost(); 

                if (changedPost == null)
                {
                    return;
                }

                changedPost.createdAt = selectedPost.createdAt;
                changedPost.id = selectedPost.id;

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


        private void OnPostItemChanged(ListViewItemEventArgs args)
        {
            selecting = true;

            currentPost = (Post)args.Value;

            SetPostRoleLimits();

            UpdateCurrentPage();
        }

        private void OnCommentItemChanged(ListViewItemEventArgs args)
        {
            currentComment = (Comment)args.Value;

            UpdateComments();
        }

        private void SetPostRoleLimits()
        {
            if (currentPost.userId == currentUser.id)
            {
                editPostBtn.Visible = true;
                deletePostBtn.Visible = true;
            }
            else if (currentUser.role == "moderator" || currentUser.role == "admin")
            {
                editPostBtn.Visible = false;
                deletePostBtn.Visible = true;
            }
            else
            {
                editPostBtn.Visible = false;
                deletePostBtn.Visible = false;
            }
        }

        private void SetCommentRoleLimits()
        {
            if (currentComment.userId == currentUser.id)
            {
                editCommentBtn.Visible = true;
                deleteCommentBtn.Visible = true;
            }
            else if (currentUser.role == "moderator" || currentUser.role == "admin")
            {
                editCommentBtn.Visible = false;
                deleteCommentBtn.Visible = true;
            }
            else
            {
                editCommentBtn.Visible = false;
                deleteCommentBtn.Visible = false;
            }
        }


        private void OnCreateButtonClick()
        {
            CreatePostDialog dialog = new CreatePostDialog();
            dialog.SetUserId(currentUser.id);

            Application.Run(dialog);

            if (dialog.accepted)
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
            dialog.SetEditorMode(currentUser);

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
                changedPost.id = post.id;

                bool isUpdated = postsRepository.Update(changedPost.id, changedPost); 

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
    
        private void OnOpenComment(ListViewItemEventArgs args)
        {
            Comment comment = (Comment)args.Value;

            OpenCommentDialog dialog = new OpenCommentDialog();
            dialog.SetComment(comment);
            //dialog.

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
                changedComment.id = comment.id;

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

        private void OnCreateCommentBtnClick()
        {
            CreateCommentDialog dialog = new CreateCommentDialog();
            dialog.SetUserId(currentUser.id);
            dialog.SetPostId(currentPost.id);
            Application.Run(dialog);

            if (dialog.accepted)
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
                    UpdateComments();
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

            if (dialog.accepted)
            {
                Comment changedComment = dialog.GetComment(); 

                if (changedComment == null)
                {
                    return;
                }

                changedComment.createdAt = selectedComment.createdAt;
                changedComment.id = selectedComment.id;
            
                bool isUpdated = commentsRepository.Update(selectedComment.id, changedComment); 

                if (isUpdated)
                {
                    UpdateComments();
                }
                else
                {
                    MessageBox.ErrorQuery("Editing comment", "Couldn't edit comment", "Ok");
                }
            }
        }
    }
}