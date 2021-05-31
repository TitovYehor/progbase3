using ProcessData;

using Terminal.Gui;

namespace TerminalGUIApp.Windows.UserWindows
{
    public class EditUserDialog : CreateUserDialog
    {
        private User currUser;

        public EditUserDialog()
        {
            this.Title = "Edit user";
            
            this.userImportedLbl.Visible = true;
            this.userImportedCaptionLbl.Visible = true;
            this.roleLbl.Visible = true;
            this.userPasswordInput.ReadOnly = true;
            this.currentRoleLbl.Visible = true;

            changePasswordBtn = new Button("Change password")
            {
                X = Pos.Right(userPasswordInput) + Pos.Percent(3),
                Y = Pos.Top(userPasswordInput),
                AutoSize = true,
            };
            changePasswordBtn.Clicked += OnOpenChangePassDialog;
            this.Add(changePasswordBtn);
        }

        public void SetUser(User user)
        {
            this.currUser = user;

            this.idLbl.Text = user.id.ToString();
            this.userUsernameInput.Text = user.username;
            this.userPasswordInput.Text = user.password;
            this.userFullnameInput.Text = user.fullname;
            this.userCreatedAtDateField.Text = user.createdAt.ToShortDateString();
            this.userImportedLbl.Text = user.imported.ToString();
            this.roleComboBox.Text = user.role;
            this.currentRoleLbl.Text = user.role;
        }

        public void SetAdminEditorMode()
        {
            this.changePasswordBtn.Visible = false;

            this.currentRoleLbl.Visible = false;
            this.roleComboBox.Visible = true;
            this.roleComboBox.SetSource(this.listOfRoles);

            this.userUsernameInput.ReadOnly = true;
            this.userPasswordInput.Visible = false;
            this.userFullnameInput.ReadOnly = true;
            this.userCreatedAtDateField.ReadOnly = true;   
        }
    
        private void OnOpenChangePassDialog()
        {
            ChangePassDialog dialog = new ChangePassDialog();
            Application.Run(dialog);
            
            if (dialog.passChanged)
            {
                this.userPasswordInput.Text = dialog.GetPass();
            }
        }
    }
}