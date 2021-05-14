using ProcessData;

namespace TerminalGUIApp
{
    public class EditUserDialog : CreateUserDialog
    {
        public EditUserDialog()
        {
            this.Title = "Edit user";
        }

        public void SetUser(User user)
        {
            this.idLbl.Text = user.id.ToString();
            this.userUsernameInput.Text = user.username;
            this.userPasswordInput.Text = user.password;
            this.userFullnameInput.Text = user.fullname;
            this.userCreatedAtDateField.Text = user.createdAt.ToShortDateString();
        }
    }
}