using System;
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
        private int pagePosts = 1;
        private int pageComments = 1;
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
        private Button createPostBtn;
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
        private Button createCommentBtn;
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

            Button backBtn = new Button("Back to menu")
            {
                X = Pos.Percent(1),
                Y = Pos.Percent(2.5f),
                AutoSize = true,
            };
            backBtn.Clicked += Application.RequestStop;
            this.Add(backBtn);

            currUserFrameView = new FrameView("Current user")
            {
                X = Pos.Center(),
                Y = Pos.Percent(5),
                Width = Dim.Fill() - Dim.Percent(33),
                Height = 3,
            };
            currUserFrameView.Add(currUserListView);
            this.Add(currUserFrameView);


            // for posts
            allPostsListView = new ListView(new List<Post>())
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            allPostsListView.OpenSelectedItem += OnOpenPost;
            allPostsListView.SelectedItemChanged += OnPostItemChanged;

            postFirstPageBtn = new Button("<<")
            {
                X = Pos.Percent(5),
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
            Label separatePostLbl = new Label("of")
            {
                X = Pos.Right(postPageLbl) + Pos.Percent(2),
                Y = Pos.Top(postPageLbl),
            };
            postTotalPagesLbl = new Label("?")
            {
                X = Pos.Right(separatePostLbl) + Pos.Percent(3),
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
            this.Add(postFirstPageBtn, postPrevPageBtn, postPageLbl, separatePostLbl, postTotalPagesLbl, postNextPageBtn, postLastPageBtn);

            Label searchPostLbl = new Label("Seeking categories - ")
            {
                X = Pos.Left(postFirstPageBtn),
                Y = Pos.Bottom(postFirstPageBtn) + Pos.Percent(5),
            };
            Label searchColumnPostLbl = new Label("Content - ")
            {
                X = Pos.Right(searchPostLbl),
                Y = Pos.Top(searchPostLbl),
            };
            postSearchInput = new TextField()
            {
                X = Pos.Right(searchColumnPostLbl) + Pos.Percent(1),
                Y = Pos.Top(searchPostLbl),
                Width = Dim.Percent(20),
            };
            postSearchInput.TextChanged += OnSearchPostChange;
            this.Add(searchPostLbl, searchColumnPostLbl, postSearchInput);

            postFrameView = new FrameView("Posts")
            {
                X = Pos.Left(postFirstPageBtn),
                Y = Pos.Bottom(searchPostLbl) + Pos.Percent(5),
                Width = Dim.Fill() - Dim.Percent(60),
                Height = pageSize + 2,
            };
            postFrameView.Add(allPostsListView);
            this.Add(postFrameView);

            createPostBtn = new Button("Create post")
            {
                X = Pos.Left(postFrameView),
                Y = Pos.Bottom(postFrameView) + Pos.Percent(5),
                AutoSize = true,
            };
            createPostBtn.Clicked += OnCreatePost;
            deletePostBtn = new Button("Delete post")
            {
                X = Pos.Right(createPostBtn) + Pos.Percent(5),
                Y = Pos.Top(createPostBtn),
            };
            deletePostBtn.Clicked += OnDeletePost;
            editPostBtn = new Button("Edit post")
            {
                X = Pos.Right(deletePostBtn) + Pos.Percent(5),
                Y = Pos.Top(deletePostBtn),
            };
            editPostBtn.Clicked += OnEditPost;
            this.Add(createPostBtn, deletePostBtn, editPostBtn);

            createCommentBtn = new Button("Сomment post")
            {
                X = Pos.Left(createPostBtn),
                Y = Pos.Bottom(createPostBtn) + Pos.Percent(5),
                AutoSize = true,
            };
            createCommentBtn.Clicked += OnCreateComment;
            this.Add(createCommentBtn);
        
            // for comments
            allCommentsListView = new ListView(new List<Comment>())
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            allCommentsListView.OpenSelectedItem += OnOpenComment;
            allCommentsListView.SelectedItemChanged += OnCommentItemChanged;

            commentFirstPageBtn = new Button("<<")
            {
                X = Pos.Center() + Pos.Percent(10),
                Y = Pos.Bottom(currUserFrameView) + Pos.Percent(15),
                Visible = false,
            };
            commentPrevPageBtn = new Button("<")
            {
                X = Pos.Right(commentFirstPageBtn) + Pos.Percent(3),
                Y = Pos.Top(commentFirstPageBtn),
                Visible = false,
            };
            commentPageLbl = new Label("?")
            {
                X = Pos.Right(commentPrevPageBtn) + Pos.Percent(3),
                Y = Pos.Top(commentPrevPageBtn),
                Width = 3,
            };
            Label separateCommentLbl = new Label("of")
            {
                X = Pos.Right(commentPageLbl) + Pos.Percent(2),
                Y = Pos.Top(commentPageLbl),
            };
            commentTotalPagesLbl = new Label("?")
            {
                X = Pos.Right(separateCommentLbl) + Pos.Percent(3),
                Y = Pos.Top(commentPageLbl),
                Width = 3,
            };
            commentNextPageBtn = new Button(">")
            {
                X = Pos.Right(commentTotalPagesLbl) + Pos.Percent(3),
                Y = Pos.Top(commentPrevPageBtn),
            };
            commentLastPageBtn = new Button(">>")
            {
                X = Pos.Right(commentNextPageBtn) + Pos.Percent(3),
                Y = Pos.Top(commentNextPageBtn),
            };
            commentFirstPageBtn.Clicked += OnFirstCommentPage;
            commentNextPageBtn.Clicked += OnNextCommentPage;
            commentPrevPageBtn.Clicked += OnPrevCommentPage;
            commentLastPageBtn.Clicked += OnLastCommentPage;
            this.Add(commentFirstPageBtn, commentPrevPageBtn, commentPageLbl, separateCommentLbl, commentTotalPagesLbl, commentNextPageBtn, commentLastPageBtn);

            Label searchCommentLbl = new Label("Seeking categories - ")
            {
                X = Pos.Left(commentFirstPageBtn),
                Y = Pos.Bottom(commentFirstPageBtn) + Pos.Percent(5),
            };
            Label searchColumnCommentLbl = new Label("Content - ")
            {
                X = Pos.Right(searchCommentLbl),
                Y = Pos.Top(searchCommentLbl),
            };
            commentSearchInput = new TextField()
            {
                X = Pos.Right(searchColumnCommentLbl) + Pos.Percent(1),
                Y = Pos.Top(searchCommentLbl),
                Width = Dim.Percent(20),
            };
            commentSearchInput.TextChanged += OnSearchCommentChange;
            this.Add(searchCommentLbl, searchColumnCommentLbl, commentSearchInput);

            commentFrameView = new FrameView("Comments")
            {
                X = Pos.Left(searchCommentLbl),
                Y = Pos.Bottom(searchCommentLbl) + Pos.Percent(5),
                Width = Dim.Percent(40),
                Height = pageSize + 2,
            };
            commentFrameView.Add(allCommentsListView);
            this.Add(commentFrameView);

            deleteCommentBtn = new Button("Delete comment")
            {
                X = Pos.Left(commentFrameView),
                Y = Pos.Bottom(commentFrameView) + Pos.Percent(5),
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

        public void SetRepositories(UserRepository userRepository, PostRepository postRepository, CommentRepository commentRepository)
        {
            this.userRepository = userRepository;

            this.postRepository = postRepository;

            this.commentRepository = commentRepository;

            UpdateCurrentPostPage();
            UpdateCurrentCommentPage();
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
            dialog.canDelete = true;
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
            else if (dialog.changed)
            {
                User changedUser = dialog.GetUser();

                changedUser.createdAt = user.createdAt;
                changedUser.id = currUser.id;

                bool isUpdated = userRepository.Update(user.id, changedUser); 

                if (isUpdated)
                {
                    MessageBox.Query("Editing user", "User information edited", "Ok");

                    currUser = changedUser;
                    currUserListView.SetSource(new List<User>(){currUser});
                }
                else
                {
                    MessageBox.ErrorQuery("Editing user", "Couldn't edit user", "Ok");
                }
            }
        }
    
        // for posts
        private void OnSearchPostChange(NStack.ustring text)
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
                    int countOfPages = postRepository.GetSearchUserPostsPagesCount(currUser.id, pageSize, postSearchValue);

                    if (pagePosts > countOfPages && pagePosts > 1)
                    {
                        pagePosts -= 1;
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
                changedPost.id = post.id;

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
        private void OnCreatePost()
        {
            CreatePostDialog dialog = new CreatePostDialog();
            dialog.SetUserId(currUser.id);
            Application.Run(dialog);

            if (dialog.accepted)
            {
                Post post = dialog.GetPost();

                if (post == null)
                {
                    return;
                }

                post.createdAt = DateTime.Now;
                int insertedId = postRepository.Insert(post);
                post.id = insertedId;

                allPostsListView.SetSource(postRepository.GetSearchPage("", postRepository.GetSearchPagesCount(pageSize, ""), pageSize));

                allPostsListView.SelectedItem = allPostsListView.Source.ToList().Count - 1;

                allPostsListView.OnOpenSelectedItem();

                UpdateCurrentPostPage();
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

                    if (pagePosts > countOfPages && pagePosts > 1)
                    {
                        pagePosts -= 1;
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
                changedPost.id = selectedPost.id;
            
                bool isUpdated = postRepository.Update(selectedPost.id, changedPost); 

                if (isUpdated)
                {
                    allPostsListView.SetSource(postRepository.GetSearchUserPostsPage(currUser.id , postSearchValue, pagePosts, pageSize));
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
            int totalPages = postRepository.GetSearchUserPostsPagesCount(currUser.id, pageSize, postSearchValue);

            if (pagePosts >= totalPages)
            {
                return;
            }

            pagePosts += 1;
            
            UpdateCurrentPostPage();
        }
        private void OnPrevPostPage()
        {
            int totalPages = postRepository.GetSearchUserPostsPagesCount(currUser.id, pageSize, postSearchValue);

            if (pagePosts == 1)
            {
                return;
            }

            pagePosts -= 1;
            
            UpdateCurrentPostPage();
        }
        private void OnLastPostPage()
        {
            int totalPages = postRepository.GetSearchUserPostsPagesCount(currUser.id, pageSize, postSearchValue);

            pagePosts = totalPages;

            UpdateCurrentPostPage();
        }
        private void OnFirstPostPage()
        {
            pagePosts = 1;

            UpdateCurrentPostPage();
        }
        private void UpdateCurrentPostPage()
        {
            int totalPages = postRepository.GetSearchUserPostsPagesCount(currUser.id, pageSize, postSearchValue);

            if (pagePosts > totalPages)
            {
                pagePosts = 1;
            }

            this.postPageLbl.Text = pagePosts.ToString();
            this.postTotalPagesLbl.Text = totalPages.ToString();

            if (!postSelecting)
            {
                this.allPostsListView.SetSource(postRepository.GetSearchUserPostsPage(currUser.id, postSearchValue, pagePosts, pageSize));

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
                    createCommentBtn.Visible = false;
                }
                else
                {
                    postFrameView.RemoveAll();
                    postFrameView.Add(allPostsListView);

                    editPostBtn.Visible = true;
                    deletePostBtn.Visible = true;
                    createCommentBtn.Visible = true;
                }
            }
            else
            {
                postSelecting = false;
            }
            

            postPrevPageBtn.Visible = (pagePosts != 1);
            postNextPageBtn.Visible = (pagePosts !< totalPages);
            postFirstPageBtn.Visible = postPrevPageBtn.Visible;
            postLastPageBtn.Visible = postNextPageBtn.Visible;
        }
    
        // for comments
        private void OnSearchCommentChange(NStack.ustring text)
        {
            commentSearchValue = commentSearchInput.Text.ToString();
            
            UpdateCurrentCommentPage();
        }
        private void OnOpenComment(ListViewItemEventArgs args)
        {            
            Comment comment = (Comment)args.Value;

            OpenCommentDialog dialog = new OpenCommentDialog();
            dialog.SetComment(comment);

            Application.Run(dialog);

            if (dialog.deleted)
            {
                bool isDeleted = commentRepository.DeleteById(comment.id); 

                if (isDeleted)
                {
                    int countOfPages = commentRepository.GetSearchUserCommentsPagesCount(currUser.id, pageSize, commentSearchValue);

                    if (pageComments > countOfPages && pageComments > 1)
                    {
                        pageComments -= 1;
                    }

                    UpdateCurrentCommentPage();
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

                bool isUpdated = commentRepository.Update(changedComment.id, changedComment); 

                if (isUpdated)
                {
                    UpdateCurrentCommentPage();
                }
                else
                {
                    MessageBox.ErrorQuery("Editing comment", "Couldn't edit comment", "Ok");
                }
            }
        }
        private void OnCreateComment()
        {
            Post currPost = (Post)allPostsListView.Source.ToList()[allPostsListView.SelectedItem];

            CreateCommentDialog dialog = new CreateCommentDialog();
            dialog.SetUserId(currUser.id);
            dialog.SetPostId(currPost.id);
            Application.Run(dialog);

            if (dialog.accepted)
            {
                Comment comment = dialog.GetComment();

                if (comment == null)
                {
                    return;
                }

                comment.createdAt = DateTime.Now;
                int insertedId = commentRepository.Insert(comment);
                comment.id = insertedId;

                allCommentsListView.SetSource(commentRepository.GetSearchPage("", commentRepository.GetSearchPagesCount(pageSize, ""), pageSize));

                allCommentsListView.SelectedItem = allCommentsListView.Source.ToList().Count - 1;

                allCommentsListView.OnOpenSelectedItem();

                UpdateCurrentCommentPage();
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
            
                if (!commentRepository.DeleteById(selectedComment.id))
                {
                    MessageBox.ErrorQuery("Deleting comment", "Couldn't delete comment", "Ok");

                    return;
                }
                else
                {
                    int countOfPages = commentRepository.GetSearchUserCommentsPagesCount(currUser.id, pageSize, commentSearchValue);

                    if (pageComments > countOfPages && pageComments > 1)
                    {
                        pageComments -= 1;
                    }

                    UpdateCurrentCommentPage();
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
            
                bool isUpdated = commentRepository.Update(selectedComment.id, changedComment); 

                if (isUpdated)
                {
                    allCommentsListView.SetSource(commentRepository.GetSearchUserCommentsPage(currUser.id, commentSearchValue, pageComments, pageSize));
                }
                else
                {
                    MessageBox.ErrorQuery("Editing comment", "Couldn't edit comment", "Ok");
                }
            }
        }
        private void OnCommentItemChanged(ListViewItemEventArgs args)
        {
            commentSelecting = true;

            UpdateCurrentCommentPage();
        }
        private void OnNextCommentPage()
        {
            int totalPages = commentRepository.GetSearchUserCommentsPagesCount(currUser.id, pageSize, commentSearchValue);

            if (pageComments >= totalPages)
            {
                return;
            }

            pageComments += 1;
            
            UpdateCurrentCommentPage();
        }
        private void OnPrevCommentPage()
        {
            int totalPages = commentRepository.GetSearchUserCommentsPagesCount(currUser.id, pageSize, commentSearchValue);

            if (pageComments == 1)
            {
                return;
            }

            pageComments -= 1;
            
            UpdateCurrentCommentPage();
        }
        private void OnLastCommentPage()
        {
            int totalPages = commentRepository.GetSearchUserCommentsPagesCount(currUser.id, pageSize, commentSearchValue);

            pageComments = totalPages;

            UpdateCurrentCommentPage();
        }
        private void OnFirstCommentPage()
        {
            pageComments = 1;

            UpdateCurrentCommentPage();
        }
        private void UpdateCurrentCommentPage()
        {
            int totalPages = commentRepository.GetSearchUserCommentsPagesCount(currUser.id, pageSize, commentSearchValue);

            if (pageComments > totalPages)
            {
                pageComments = 1;
            }

            this.commentPageLbl.Text = pageComments.ToString();
            this.commentTotalPagesLbl.Text = totalPages.ToString();

            if (!commentSelecting)
            {
                this.allCommentsListView.SetSource(commentRepository.GetSearchUserCommentsPage(currUser.id, commentSearchValue, pageComments, pageSize));

                if (allCommentsListView.Source.ToList().Count == 0)
                {
                    nullReferenceLbl = new Label("No records found")
                    {
                        X = Pos.Percent(45),
                        Y = Pos.Percent(50),
                    };
                    commentFrameView.RemoveAll();
                    commentFrameView.Add(nullReferenceLbl);

                    editCommentBtn.Visible = false;
                    deleteCommentBtn.Visible = false;
                }
                else
                {
                    commentFrameView.RemoveAll();
                    commentFrameView.Add(allCommentsListView);

                    editCommentBtn.Visible = true;
                    deleteCommentBtn.Visible = true;
                }
            }
            else
            {
                commentSelecting = false;
            }
            

            commentPrevPageBtn.Visible = (pageComments != 1);
            commentNextPageBtn.Visible = (pageComments !< totalPages);
            commentFirstPageBtn.Visible = commentPrevPageBtn.Visible;
            commentLastPageBtn.Visible = commentNextPageBtn.Visible;
        }
    }
}