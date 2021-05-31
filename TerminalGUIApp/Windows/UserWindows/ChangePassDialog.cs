using Terminal.Gui;

using ProcessData;

namespace TerminalGUIApp.Windows.UserWindows
{
    public class ChangePassDialog : Dialog
    {
        public bool passChanged = false;

        private TextField userPasswordInput;
        private TextField userConfirmPasswordInput;

        private string newPass;


        public ChangePassDialog()
        {
            Label userPasswordLbl = new Label("Password: ")
            {
                X = Pos.Percent(10),
                Y = Pos.Percent(10),
            };
            userPasswordInput = new TextField("")
            {
                X = Pos.Right(userPasswordLbl) + Pos.Percent(5),
                Y = Pos.Top(userPasswordLbl),
                Width = Dim.Percent(10),
                Secret = true,
            };
            Label userConfirmPasswordLbl = new Label("Confirm password: ")
            {
                X = Pos.Left(userPasswordLbl),
                Y = Pos.Bottom(userPasswordLbl) + Pos.Percent(5),
            };
            userConfirmPasswordInput = new TextField("")
            {
                X = Pos.Right(userConfirmPasswordLbl) + Pos.Percent(5),
                Y = Pos.Top(userConfirmPasswordLbl),
                Width = Dim.Percent(10),
                Secret = true,
            };
            this.Add(userPasswordLbl, userPasswordInput, userConfirmPasswordLbl, userConfirmPasswordInput);

            Button changePassBtn = new Button("Accept change")
            {
                X = Pos.Left(userPasswordLbl),
                Y = Pos.Bottom(userConfirmPasswordLbl) + Pos.Percent(5),
                AutoSize = true,
            };
            changePassBtn.Clicked += OnChangePassword;
            this.Add(changePassBtn);

            Button backBtn = new Button("Back")
            {
                X = Pos.Left(changePassBtn),
                Y = Pos.Bottom(changePassBtn) + Pos.Percent(5),
                AutoSize = true,
            };
            backBtn.Clicked += Application.RequestStop;
            this.Add(backBtn);
        }


        public string GetPass()
        {
            return newPass;
        }

        private void OnChangePassword()
        {
            if (userPasswordInput.Text != userConfirmPasswordInput.Text)
            {
                MessageBox.ErrorQuery("Changing password", "Password mismatch. Please re-check", "Ok");
            }
            else
            {
                string pass = userPasswordInput.Text.ToString(); 

                newPass = HashModule.Hash(pass);

                MessageBox.Query("Changing password", "Password changed", "Ok");

                passChanged = true;
            
                Application.RequestStop();
            }
        }
    }
}