using Terminal.Gui;

using ProcessData;

namespace TerminalGUIApp.Windows.CommentWindows
{
    public class CreateCommentDialog : Dialog
    {
        public bool accepted;

        protected Label idLbl;
        protected TextField commentContentInput; 
        protected DateField commentCreatedAtDateField; 
        protected TextField commentUserIdInput;
        protected TextField commentPostIdInput;



        public CreateCommentDialog()
        {
            this.Title = "Create comment";

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


            Button okBtn = new Button("Ok");
            okBtn.Clicked += OnCreateDialogSubmit;

            Button cancelBtn = new Button("Cancel");
            cancelBtn.Clicked += OnCreateDialogCanceled;

            this.AddButton(cancelBtn);
            this.AddButton(okBtn);
        }


        public Comment GetComment()
        {
            int userId;
            bool tryUserId = int.TryParse(commentUserIdInput.Text.ToString(), out userId);

            if (!tryUserId || userId < 0)
            {
                MessageBox.ErrorQuery("Creating user", "Incorrect userId value. Must be non-negative integer", "Ok");
                return null;
            }

            int postId;
            bool tryPostId = int.TryParse(commentPostIdInput.Text.ToString(), out postId);

            if (!tryPostId || postId < 0)
            {
                MessageBox.ErrorQuery("Creating user", "Incorrect postId value. Must be non-negative integer", "Ok");
                return null;
            }

            return new Comment()
            {
                content = commentContentInput.Text.ToString(),
                createdAt = commentCreatedAtDateField.Date, 
                userId = userId,
                postId = postId,
            };
        }

        public void SetUserId(int userId)
        {
            this.commentUserIdInput.Text = userId.ToString();
        }
        public void SetPostId(int postId)
        {
            this.commentPostIdInput.Text = postId.ToString();
        }

        private void OnCreateDialogCanceled()
        {
            accepted = false;

            Application.RequestStop();
        }

        private void OnCreateDialogSubmit()
        {
            accepted = true;

            Application.RequestStop();
        }
    }
}