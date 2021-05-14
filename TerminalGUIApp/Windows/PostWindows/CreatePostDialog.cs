using Terminal.Gui;

using ProcessData;

namespace TerminalGUIApp.Windows.PostWindows
{
    public class CreatePostDialog : Dialog
    {
        public bool canceled;

        protected Label idLbl;
        protected TextField postContentInput; 
        protected DateField postCreatedAtDateField; 
        protected TextField postUserIdInput;



        public CreatePostDialog()
        {
            this.Title = "Create post";

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
            };
            this.Add(postUserIdLbl, postUserIdInput);


            Button okBtn = new Button("Ok");
            okBtn.Clicked += OnCreateDialogSubmit;

            Button cancelBtn = new Button("Cancel");
            cancelBtn.Clicked += OnCreateDialogCanceled;

            this.AddButton(cancelBtn);
            this.AddButton(okBtn);
        }


        public Post GetPost()
        {
            int userId;
            bool tryUserId = int.TryParse(postUserIdInput.Text.ToString(), out userId);

            if (!tryUserId || userId < 0)
            {
                MessageBox.ErrorQuery("Creating user", "Incorrect userId value. Must be non-negative integer", "Ok");
                return null;
            }

            return new Post()
            {
                content = postContentInput.Text.ToString(),
                createdAt = postCreatedAtDateField.Date, 
                userId = userId,
            };
        }


        private void OnCreateDialogCanceled()
        {
            canceled = true;

            Application.RequestStop();
        }

        private void OnCreateDialogSubmit()
        {
            canceled = false;

            Application.RequestStop();
        }
    }
}