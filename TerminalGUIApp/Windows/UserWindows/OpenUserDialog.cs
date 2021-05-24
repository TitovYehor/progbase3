using Terminal.Gui;

using ProcessData;

namespace TerminalGUIApp.Windows.UserWindows
{
    public class OpenUserDialog : Dialog
    {
        public bool deleted;

        public bool changed;

        protected User user;

        private Label idLbl;
        private TextField userUsernameInput;  
        private TextField userPasswordInput;
        private TextField userFullnameInput;
        private DateField userCreatedAtDateField;
        private Label userImportedLbl;



        public OpenUserDialog()
        {
            this.Title = "User detail information";

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
                ReadOnly = true,
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
                ReadOnly = true,
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
                ReadOnly = true,
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

            Label userImportedCaptionLbl = new Label("Imported: ")
            {
                X = Pos.Left(idLabelLbl),
                Y = Pos.Top(idLabelLbl) + Pos.Percent(50),
            };
            userImportedLbl = new Label("?")
            {
                X = Pos.Percent(20),
                Y = Pos.Top(userImportedCaptionLbl),
                Width = Dim.Percent(25),
            };
            this.Add(userImportedCaptionLbl, userImportedLbl);
            
            
            Button editBtn = new Button("Edit")
            {
                X = Pos.Percent(90),
                Y = Pos.Percent(5),
            };
            editBtn.Clicked += OnUserEdit;
            Button deleteBtn = new Button("Delete")
            {
                X = editBtn.X,
                Y = Pos.Percent(10),
            };
            deleteBtn.Clicked += OnUserDelete;
            this.Add(editBtn, deleteBtn);       


            Button backBtn = new Button("Back");
            backBtn.Clicked += OnOpenUserDialogBack;

            this.AddButton(backBtn);
        }


        public void SetUser(User user)
        {
            this.user = user;

            this.idLbl.Text = user.id.ToString();
            this.userUsernameInput.Text = user.username;
            this.userPasswordInput.Text = user.password;
            this.userFullnameInput.Text = user.fullname;
            this.userCreatedAtDateField.Text = user.createdAt.ToShortDateString();
            this.userImportedLbl.Text = user.imported.ToString();
        }

        public User GetUser()
        {
            return user;
        }


        private void OnOpenUserDialogBack()
        {
            Application.RequestStop();
        }
    
        private void OnUserDelete()
        {
            int index = MessageBox.Query("Deleting user", "Confirm deleting", "No", "Yes");

            if (index == 1)
            {
                deleted = true;

                Application.RequestStop();
            }
        }
    
        private void OnUserEdit()
        {
            EditUserDialog dialog = new EditUserDialog();
            dialog.SetUser(user);

            Application.Run(dialog);

            if (dialog.accepted)
            {
                User changedUser = dialog.GetUser();

                if (changedUser == null)
                {
                    return;
                } 

                changedUser.createdAt = user.createdAt;

                this.changed = true;
                this.SetUser(changedUser);
            }
        }
    }
}