using Terminal.Gui;

using ProcessData;

namespace TerminalGUIApp.Windows.PostWindows
{
    public class OpenPostDialog : Dialog
    {
        private bool isModeratorMode;

        public bool deleted;

        public bool changed;

        protected Post post;

        private Label idLbl;
        private TextField postContentInput; 
        private DateField postCreatedAtDateField; 
        private TextField postUserIdInput;

        private Button editBtn;
        private Button deleteBtn;



        public OpenPostDialog()
        {
            this.Title = "Post detail information";

            Label idLabelLbl = new Label("id: ")
            {
                X = Pos.Percent(5),
                Y = Pos.Percent(5),
            };
            idLbl = new Label("Auto-generated value")
            {
                X = Pos.Percent(20),
                Y = Pos.Top(idLabelLbl),
            };           
            this.Add(idLabelLbl, idLbl);

            Label postContentLbl = new Label("Content: ")
            {
                X = Pos.Left(idLabelLbl),
                Y = Pos.Top(idLabelLbl) + Pos.Percent(10),
            };
            postContentInput = new TextField("")
            {
                X = Pos.Left(idLbl),
                Y = Pos.Top(postContentLbl),
                Width = Dim.Percent(25),
                ReadOnly = true,
            };
            this.Add(postContentLbl, postContentInput);

            Label postCreatedAtLbl = new Label("CreatedAt: ")
            {
                X = Pos.Left(idLabelLbl),
                Y = Pos.Top(idLabelLbl) + Pos.Percent(20),
            };
            postCreatedAtDateField = new DateField()
            {
                X = Pos.Left(idLbl),
                Y = Pos.Top(postCreatedAtLbl),
                Width = Dim.Percent(25),
                IsShortFormat = false,
                ReadOnly = true,
            };
            this.Add(postCreatedAtLbl, postCreatedAtDateField);

            Label postUserIdLbl = new Label("User id: ")
            {
                X = Pos.Left(idLabelLbl),
                Y = Pos.Top(idLabelLbl) + Pos.Percent(30),
            };
            postUserIdInput = new TextField("")
            {
                X = Pos.Left(idLbl),
                Y = Pos.Top(postUserIdLbl),
                Width = Dim.Percent(25),
                ReadOnly = true,
            };
            this.Add(postUserIdLbl, postUserIdInput);
            
            
            editBtn = new Button("Edit")
            {
                X = Pos.Percent(90),
                Y = Pos.Percent(5),
            };
            editBtn.Clicked += OnPostEdit;
            deleteBtn = new Button("Delete")
            {
                X = editBtn.X,
                Y = Pos.Percent(10),
            };
            deleteBtn.Clicked += OnPostDelete;
            this.Add(editBtn, deleteBtn);       


            Button backBtn = new Button("Back");
            backBtn.Clicked += OnOpenPostDialogBack;

            this.AddButton(backBtn);
        }


        public void SetPost(Post post)
        {
            this.post = post;

            this.idLbl.Text = post.id.ToString();
            this.postContentInput.Text = post.content;
            this.postCreatedAtDateField.Text = post.createdAt.ToShortDateString();
            this.postUserIdInput.Text = post.userId.ToString();
        }

        public void SetEditorMode(User user)
        {
            if (user.id == post.userId)
            {
                this.editBtn.Visible = true;

                this.deleteBtn.Visible = true;
            }
            else if (user.role == "moderator" || user.role == "admin")
            {
                this.isModeratorMode = true;

                this.editBtn.Visible = false;

                this.deleteBtn.Visible = true;
            }
            else
            {
                this.editBtn.Visible = false;

                this.deleteBtn.Visible = false;
            }
        }

        public Post GetPost()
        {
            return post;
        }


        private void OnOpenPostDialogBack()
        {
            Application.RequestStop();
        }
    
        private void OnPostDelete()
        {
            int index = MessageBox.Query("Deleting post", "Confirm deleting", "No", "Yes");

            if (index == 1)
            {
                deleted = true;

                Application.RequestStop();
            }
        }
    
        private void OnPostEdit()
        {
            EditPostDialog dialog = new EditPostDialog();
            dialog.SetPost(post);

            Application.Run(dialog);

            if (dialog.accepted)
            {
                Post changedPost = dialog.GetPost();

                if (changedPost == null)
                {
                    return;
                } 

                changedPost.createdAt = post.createdAt;
                changedPost.id = post.id;

                this.changed = true;
                this.SetPost(changedPost);
            }
        }
    }
}