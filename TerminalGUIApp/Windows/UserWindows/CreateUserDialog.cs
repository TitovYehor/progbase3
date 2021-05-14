using Terminal.Gui;

using ProcessData;

namespace TerminalGUIApp.Windows.UserWindows
{
    public class CreateUserDialog : Dialog
    {
        public bool canceled;

        protected Label idLbl;
        protected TextField userUsernameInput;  
        protected TextField userPasswordInput;
        protected TextField userFullnameInput;
        protected DateField userCreatedAtDateField;



        public CreateUserDialog()
        {
            this.Title = "Create user";

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

            Label userUsernameLbl = new Label("Username: ")
            {
                X = Pos.Left(idLabelLbl),
                Y = Pos.Top(idLabelLbl) + Pos.Percent(10),
            };
            userUsernameInput = new TextField("")
            {
                X = Pos.Left(idLbl),
                Y = Pos.Top(userUsernameLbl),
                Width = Dim.Percent(25),
            };
            this.Add(userUsernameLbl, userUsernameInput);

            Label userPasswordLbl = new Label("Password: ")
            {
                X = Pos.Left(idLabelLbl),
                Y = Pos.Top(idLabelLbl) + Pos.Percent(20),
            };
            userPasswordInput = new TextField("")
            {
                X = Pos.Left(idLbl),
                Y = Pos.Top(userPasswordLbl),
                Width = Dim.Percent(25),
                Secret = true,
            };
            this.Add(userPasswordLbl, userPasswordInput);

            Label userFullnameLbl = new Label("Fullname: ")
            {
                X = Pos.Left(idLabelLbl),
                Y = Pos.Top(idLabelLbl) + Pos.Percent(30),
            };
            userFullnameInput = new TextField("")
            {
                X = Pos.Left(idLbl),
                Y = Pos.Top(userFullnameLbl),
                Width = Dim.Percent(25),
            };
            this.Add(userFullnameLbl, userFullnameInput);

            Label userCreatedAtLbl = new Label("CreatedAt: ")
            {
                X = Pos.Left(idLabelLbl),
                Y = Pos.Top(idLabelLbl) + Pos.Percent(40),
            };
            userCreatedAtDateField = new DateField()
            {
                X = Pos.Percent(20),
                Y = Pos.Top(userCreatedAtLbl),
                Width = Dim.Percent(25),
                IsShortFormat = false,
                ReadOnly = true,
            };
            this.Add(userCreatedAtLbl, userCreatedAtDateField);


            Button okBtn = new Button("Ok");
            okBtn.Clicked += OnCreateDialogSubmit;

            Button cancelBtn = new Button("Cancel");
            cancelBtn.Clicked += OnCreateDialogCanceled;

            this.AddButton(cancelBtn);
            this.AddButton(okBtn);
        }


        public User GetUser()
        {
            return new User()
            {
                username = userUsernameInput.Text.ToString(),
                password = userPasswordInput.Text.ToString(),
                fullname = userFullnameInput.Text.ToString(),
                createdAt = userCreatedAtDateField.Date, 
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