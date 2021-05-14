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
            };
            this.Add(commentPostIdLbl, commentPostIdInput);
            
            
            Button editBtn = new Button("Edit")
            {
                X = Pos.Percent(90),
                Y = Pos.Percent(5),
            };
            editBtn.Clicked += OnCommentEdit;
            Button deleteBtn = new Button("Delete")
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

            if (!dialog.canceled)
            {
                Comment changedComment = dialog.GetComment();

                if (changedComment == null)
                {
                    return;
                } 

                this.changed = true;
                this.SetComment(changedComment);
            }
        }
    }
}