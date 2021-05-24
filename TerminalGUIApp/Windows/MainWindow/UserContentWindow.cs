using System.Collections.Generic;
using Terminal.Gui;

using ProcessData;
using TerminalGUIApp.Windows.UserWindows;
using TerminalGUIApp.Windows.PostWindows;
using TerminalGUIApp.Windows.CommentWindows;

namespace TerminalGUIApp.Windows.MainWindow
{
    public class UserContentWindow : Window
    {
        private User currUser;

        public bool deleted;

        private UserRepository userRepository;
        private PostRepository postRepository;
        private CommentRepository commentRepository;

        private ListView currUserListView;
        private FrameView currUserFrameView;

        private int pageSize = 10;
        private int page = 1;
        private Label nullReferenceLbl = new Label();

        // for posts
        private string postSearchValue = "";
        private bool postSelecting = false;

        private ListView allPostsListView;

        private Label postPageLbl;
        private Label postTotalPagesLbl;
        private TextField postSearchInput;
        private FrameView postFrameView;
        private Button postPrevPageBtn;
        private Button postNextPageBtn;
        private Button postFirstPageBtn;
        private Button postLastPageBtn;
        private Button deletePostBtn;
        private Button editPostBtn;

        // for comments
        private string commentSearchValue = "";
        private bool commentSelecting = false;

        private ListView allCommentsListView;

        private Label commentPageLbl;
        private Label commentTotalPagesLbl;
        private TextField commentSearchInput;
        private FrameView commentFrameView;
        private Button commentPrevPageBtn;
        private Button commentNextPageBtn;
        private Button commentFirstPageBtn;
        private Button commentLastPageBtn;
        private Button deleteCommentBtn;
        private Button editCommentBtn;



        public UserContentWindow()
        {
            currUserListView = new ListView(new List<User>())
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            currUserListView.OpenSelectedItem += OnOpenUser;

            currUserFrameView = new FrameView("Current user")
            {
                X = Pos.Center(),
                Y = Pos.Percent(5),
                Width = Dim.Fill() - Dim.Percent(33),
                Height = 3,
            };
            currUserFrameView.Add(currUserListView);
            this.Add(currUserFrameView);

            allPostsListView = new ListView(new List<Post>())
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            allPostsListView.OpenSelectedItem += OnOpenPost;
            allPostsListView.SelectedItemChanged += OnPostItemChanged;

            postFirstPageBtn = new Button("<<")
            {
                X = Pos.Percent(1),
                Y = Pos.Bottom(currUserFrameView) + Pos.Percent(15),
                Visible = false,
            };
            postPrevPageBtn = new Button("<")
            {
                X = Pos.Right(postFirstPageBtn) + Pos.Percent(3),
                Y = Pos.Top(postFirstPageBtn),
                Visible = false,
            };
            postPageLbl = new Label("?")
            {
                X = Pos.Right(postPrevPageBtn) + Pos.Percent(3),
                Y = Pos.Top(postPrevPageBtn),
                Width = 3,
            };
            Label separateLbl = new Label("of")
            {
                X = Pos.Right(postPageLbl) + Pos.Percent(2),
                Y = Pos.Top(postPageLbl),
            };
            postTotalPagesLbl = new Label("?")
            {
                X = Pos.Right(separateLbl) + Pos.Percent(3),
                Y = Pos.Top(postPageLbl),
                Width = 3,
            };
            postNextPageBtn = new Button(">")
            {
                X = Pos.Right(postTotalPagesLbl) + Pos.Percent(3),
                Y = Pos.Top(postPrevPageBtn),
            };
            postLastPageBtn = new Button(">>")
            {
                X = Pos.Right(postNextPageBtn) + Pos.Percent(3),
                Y = Pos.Top(postNextPageBtn),
            };
            postFirstPageBtn.Clicked += OnFirstPostPage;
            postNextPageBtn.Clicked += OnNextPostPage;
            postPrevPageBtn.Clicked += OnPrevPostPage;
            postLastPageBtn.Clicked += OnLastPostPage;
            this.Add(postFirstPageBtn, postPrevPageBtn, postPageLbl, separateLbl, postTotalPagesLbl, postNextPageBtn, postLastPageBtn);

            Label searchLbl = new Label("Seeking categories - ")
            {
                X = Pos.Left(postFirstPageBtn),
                Y = Pos.Bottom(postFirstPageBtn) + Pos.Percent(5),
            };
            Label chooseSearchColumn = new Label("Content - ")
            {
                X = Pos.Right(searchLbl),
                Y = Pos.Top(searchLbl),
            };
            postSearchInput = new TextField()
            {
                X = Pos.Right(chooseSearchColumn) + Pos.Percent(1),
                Y = Pos.Top(searchLbl),
                Width = Dim.Percent(20),
            };
            postSearchInput.TextChanged += OnSearchChange;
            this.Add(searchLbl, chooseSearchColumn, postSearchInput);

            postFrameView = new FrameView("Posts")
            {
                X = Pos.Percent(1),
                Y = Pos.Bottom(searchLbl) + Pos.Percent(5),
                Width = Dim.Fill() - Dim.Percent(60),
                Height = pageSize + 2,
            };
            postFrameView.Add(allPostsListView);
            this.Add(postFrameView);

            deletePostBtn = new Button("Delete post")
            {
                X = Pos.Left(postFrameView),
                Y = Pos.Bottom(postFrameView) + Pos.Percent(5),
            };
            deletePostBtn.Clicked += OnDeletePost;
            editPostBtn = new Button("Edit post")
            {
                X = Pos.Right(deletePostBtn) + Pos.Percent(10),
                Y = Pos.Top(deletePostBtn),
            };
            editPostBtn.Clicked += OnEditPost;
            this.Add(deletePostBtn, editPostBtn);
        }

        public void SetRepositories(UserRepository userRepository, PostRepository postRepository, CommentRepository commentRepository)
        {
            this.userRepository = userRepository;

            this.postRepository = postRepository;

            this.commentRepository = commentRepository;

            UpdateCurrentPostPage();
        }

        public void SetCurrentUser(User user)
        {
            this.currUser = user;

            this.currUserListView.SetSource(new List<User>(){currUser});
            this.currUserFrameView.Add(this.currUserListView);
        }
    
        private void OnOpenUser(ListViewItemEventArgs args)
        {            
            User user = (User)args.Value;

            OpenUserDialog dialog = new OpenUserDialog();
            dialog.canEdit = true;
            dialog.SetUser(user);

            Application.Run(dialog);

            if (dialog.deleted)
            {
                bool isDeleted = userRepository.DeleteByUsername(user.username); 

                if (isDeleted)
                {
                    MessageBox.Query("Deleting user", "Account deleted.", "Ok");
                    
                    deleted = true;

                    Application.RequestStop();
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

                bool isUpdated = userRepository.Update(user.id, changedUser); 

                if (isUpdated)
                {
                    MessageBox.Query("Editing user", "User information edited", "Ok");
                }
                else
                {
                    MessageBox.ErrorQuery("Editing user", "Couldn't edit user", "Ok");
                }
            }
        }
    
        // for posts
        private void OnSearchChange(NStack.ustring text)
        {
            postSearchValue = postSearchInput.Text.ToString();
            
            UpdateCurrentPostPage();
        }
        private void OnOpenPost(ListViewItemEventArgs args)
        {            
            Post post = (Post)args.Value;

            OpenPostDialog dialog = new OpenPostDialog();
            dialog.SetPost(post);

            Application.Run(dialog);

            if (dialog.deleted)
            {
                bool isDeleted = postRepository.DeleteById(post.id); 

                if (isDeleted)
                {
                    int countOfPages = postRepository.GetSearchPagesCount(pageSize, postSearchValue);

                    if (page > countOfPages && page > 1)
                    {
                        page -= 1;
                    }

                    UpdateCurrentPostPage();
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

                bool isUpdated = postRepository.Update(post.id, changedPost); 

                if (isUpdated)
                {
                    UpdateCurrentPostPage();
                }
                else
                {
                    MessageBox.ErrorQuery("Editing post", "Couldn't edit post", "Ok");
                }
            }
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
            
                if (!postRepository.DeleteById(selectedPost.id))
                {
                    MessageBox.ErrorQuery("Deleting post", "Couldn't delete post", "Ok");

                    return;
                }
                else
                {
                    int countOfPages = postRepository.GetSearchUserPostsPagesCount(currUser.id, pageSize, postSearchValue);

                    if (page > countOfPages && page > 1)
                    {
                        page -= 1;
                    }

                    UpdateCurrentPostPage();
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
            
                bool isUpdated = postRepository.Update(selectedPost.id, changedPost); 

                if (isUpdated)
                {
                    allPostsListView.SetSource(postRepository.GetSearchUserPostsPage(currUser.id , postSearchValue, page, pageSize));
                }
                else
                {
                    MessageBox.ErrorQuery("Editing post", "Couldn't edit post", "Ok");
                }
            }
        }
        private void OnPostItemChanged(ListViewItemEventArgs args)
        {
            postSelecting = true;

            UpdateCurrentPostPage();
        }
        private void OnNextPostPage()
        {
            int totalPages = postRepository.GetSearchPagesCount(pageSize, postSearchValue);

            if (page >= totalPages)
            {
                return;
            }

            page += 1;
            
            UpdateCurrentPostPage();
        }
        private void OnPrevPostPage()
        {
            int totalPages = postRepository.GetSearchPagesCount(pageSize, postSearchValue);

            if (page == 1)
            {
                return;
            }

            page -= 1;
            
            UpdateCurrentPostPage();
        }
        private void OnLastPostPage()
        {
            int totalPages = postRepository.GetSearchPagesCount(pageSize, postSearchValue);

            page = totalPages;

            UpdateCurrentPostPage();
        }
        private void OnFirstPostPage()
        {
            page = 1;

            UpdateCurrentPostPage();
        }
        private void UpdateCurrentPostPage()
        {
            int totalPages = postRepository.GetSearchUserPostsPagesCount(currUser.id, pageSize, postSearchValue);

            if (page > totalPages)
            {
                page = 1;
            }

            this.postPageLbl.Text = page.ToString();
            this.postTotalPagesLbl.Text = totalPages.ToString();

            if (!postSelecting)
            {
                this.allPostsListView.SetSource(postRepository.GetSearchUserPostsPage(currUser.id, postSearchValue, page, pageSize));

                if (allPostsListView.Source.ToList().Count == 0)
                {
                    nullReferenceLbl = new Label("No records found")
                    {
                        X = Pos.Percent(45),
                        Y = Pos.Percent(50),
                    };
                    postFrameView.RemoveAll();
                    postFrameView.Add(nullReferenceLbl);

                    editPostBtn.Visible = false;
                    deletePostBtn.Visible = false;
                }
                else
                {
                    postFrameView.RemoveAll();
                    postFrameView.Add(allPostsListView);

                    editPostBtn.Visible = true;
                    deletePostBtn.Visible = true;
                }
            }
            else
            {
                postSelecting = false;
            }
            

            postPrevPageBtn.Visible = (page != 1);
            postNextPageBtn.Visible = (page !< totalPages);
            postFirstPageBtn.Visible = postPrevPageBtn.Visible;
            postLastPageBtn.Visible = postNextPageBtn.Visible;
        }
    
    
    }
}