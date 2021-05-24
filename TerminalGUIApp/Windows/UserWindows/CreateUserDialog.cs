using System.Collections.Generic;
using Terminal.Gui;

using ProcessData;

namespace TerminalGUIApp.Windows.UserWindows
{
    public class CreateUserDialog : Dialog
    {
        public bool accepted = false;

        protected Label idLbl;
        protected TextField userUsernameInput;  
        protected TextField userPasswordInput;
        protected TextField userFullnameInput;
        protected DateField userCreatedAtDateField;
        protected Label userImportedCaptionLbl;
        protected Label userImportedLbl;
        protected Label roleLbl;
        protected List<string> listOfRoles = new List<string>(){"user", "moderator", "admin"};
        protected ComboBox roleComboBox;



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

            userImportedCaptionLbl = new Label("Imported: ")
            {
                X = Pos.Left(idLabelLbl),
                Y = Pos.Top(idLabelLbl) + Pos.Percent(50),
                Visible = false,
            };
            userImportedLbl = new Label("?")
            {
                X = Pos.Percent(20),
                Y = Pos.Top(userImportedCaptionLbl),
                Width = Dim.Percent(25),
                Visible = false,
            };
            this.Add(userImportedCaptionLbl, userImportedLbl);

            roleLbl = new Label("Role: ")
            {
                X = Pos.Left(idLabelLbl),
                Y = Pos.Top(idLabelLbl) + Pos.Percent(60),
                Visible = false,
            };
            roleComboBox = new ComboBox("")
            {
                X = Pos.Percent(20),
                Y = Pos.Top(roleLbl),
                Width = Dim.Percent(10),
                Height = Dim.Fill(),
                Visible = false,
            };
            roleComboBox.SetSource(listOfRoles);
            this.Add(roleLbl, roleComboBox);

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
                id = int.Parse(idLbl.Text.ToString()),
                username = userUsernameInput.Text.ToString(),
                password = userPasswordInput.Text.ToString(),
                fullname = userFullnameInput.Text.ToString(),
                createdAt = userCreatedAtDateField.Date, 
                imported = (userImportedLbl.Text.ToString() == "True") ? true : false,
                role = roleComboBox.Text.ToString(),
            };
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