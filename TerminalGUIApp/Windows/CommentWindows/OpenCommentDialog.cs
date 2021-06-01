using Terminal.Gui;

using ProcessData;

namespace TerminalGUIApp.Windows.CommentWindows
{
    public class OpenCommentDialog : Dialog
    {
        public bool deleted;

        public bool changed;

        protected Comment comment;

        private Label idLbl;
        private TextField commentContentInput; 
        private DateField commentCreatedAtDateField; 
        private TextField commentUserIdInput;
        private TextField commentPostIdInput;
        private Button editBtn;
        private Button deleteBtn;


        public OpenCommentDialog()
        {
            this.Title = "Comment detail information";

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

            Label commentContentLbl = new Label("Content: ")
            {
                X = Pos.Left(idLabelLbl),
                Y = Pos.Top(idLabelLbl) + Pos.Percent(10),
            };
            commentContentInput = new TextField("")
            {
                X = Pos.Left(idLbl),
                Y = Pos.Top(commentContentLbl),
                Width = Dim.Percent(25),
                ReadOnly = true,
            };
            this.Add(commentContentLbl, commentContentInput);

            Label commentCreatedAtLbl = new Label("CreatedAt: ")
            {
                X = Pos.Left(idLabelLbl),
                Y = Pos.Top(idLabelLbl) + Pos.Percent(20),
            };
            commentCreatedAtDateField = new DateField()
            {
                X = Pos.Left(idLbl),
                Y = Pos.Top(commentCreatedAtLbl),
                Width = Dim.Percent(25),
                IsShortFormat = false,
                ReadOnly = true,
            };
            this.Add(commentCreatedAtLbl, commentCreatedAtDateField);

            Label commentUserIdLbl = new Label("User id: ")
            {
                X = Pos.Left(idLabelLbl),
                Y = Pos.Top(idLabelLbl) + Pos.Percent(30),
            };
            commentUserIdInput = new TextField("")
            {
                X = Pos.Left(idLbl),
                Y = Pos.Top(commentUserIdLbl),
                Width = Dim.Percent(25),
                ReadOnly = true,
            };
            this.Add(commentUserIdLbl, commentUserIdInput);

            Label commentPostIdLbl = new Label("Post id: ")
            {
                X = Pos.Left(idLabelLbl),
                Y = Pos.Top(idLabelLbl) + Pos.Percent(40),
            };
            commentPostIdInput = new TextField("")
            {
                X = Pos.Left(idLbl),
                Y = Pos.Top(commentPostIdLbl),
                Width = Dim.Percent(25),
                ReadOnly = true,
            };
            this.Add(commentPostIdLbl, commentPostIdInput);
            
            
            editBtn = new Button("Edit")
            {
                X = Pos.Percent(90),
                Y = Pos.Percent(5),
            };
            editBtn.Clicked += OnCommentEdit;
            deleteBtn = new Button("Delete")
            {
                X = editBtn.X,
                Y = Pos.Percent(10),
            };
            deleteBtn.Clicked += OnCommentDelete;
            this.Add(editBtn, deleteBtn);       


            Button backBtn = new Button("Back");
            backBtn.Clicked += OnOpenCommentDialogBack;

            this.AddButton(backBtn);
        }


        public void SetComment(Comment comment)
        {
            this.comment = comment;

            this.idLbl.Text = comment.id.ToString();
            this.commentContentInput.Text = comment.content;
            this.commentCreatedAtDateField.Text = comment.createdAt.ToShortDateString();
            this.commentUserIdInput.Text = comment.userId.ToString();
            this.commentPostIdInput.Text = comment.postId.ToString();
        }

        public void SetUser(User user)
        {
            if (user.id == comment.userId)
            {
                editBtn.Visible = true;
                deleteBtn.Visible = true;
            }
            else if (user.role == "moderator" || user.role == "admin")
            {
                editBtn.Visible = false;
                deleteBtn.Visible = true;
            }
            else
            {
                editBtn.Visible = false;
                deleteBtn.Visible = false;
            }
        }

        public Comment GetComment()
        {
            return comment;
        }


        private void OnOpenCommentDialogBack()
        {
            Application.RequestStop();
        }
    
        private void OnCommentDelete()
        {
            int index = MessageBox.Query("Deleting comment", "Confirm deleting", "No", "Yes");

            if (index == 1)
            {
                deleted = true;

                Application.RequestStop();
            }
        }
    
        private void OnCommentEdit()
        {
            EditCommentDialog dialog = new EditCommentDialog();
            dialog.SetComment(comment);

            Application.Run(dialog);

            if (dialog.accepted)
            {
                Comment changedComment = dialog.GetComment();

                if (changedComment == null)
                {
                    return;
                } 

                changedComment.createdAt = comment.createdAt;
                changedComment.id = comment.id;

                this.changed = true;
                this.SetComment(changedComment);
            }
        }
    }
}