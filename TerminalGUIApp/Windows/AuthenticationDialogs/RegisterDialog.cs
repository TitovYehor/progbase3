using System;
using Terminal.Gui;

using ProcessData;

namespace TerminalGUIApp.Windows.AuthenticationDialogs
{
    public class RegisterDialog : Dialog
    {
        private UserRepository userRepository;
        private Authentication authentication;

        private TextField userUsernameInput;
        private TextField userPasswordInput;
        private TextField userConfirmPasswordInput;
        private TextField userFullnameInput;




        public RegisterDialog()
        {
            this.Title = "Registration";

            Label userUsernameLbl = new Label("Username: ")
            {
                X = Pos.Percent(10),
                Y = Pos.Percent(10),
            };
            userUsernameInput = new TextField("")
            {
                X = Pos.Right(userUsernameLbl) + Pos.Percent(5),
                Y = Pos.Top(userUsernameLbl),
                Width = Dim.Percent(25),
            };
            this.Add(userUsernameLbl, userUsernameInput);

            Label userPasswordLbl = new Label("Password: ")
            {
                X = Pos.Left(userUsernameLbl),
                Y = Pos.Bottom(userUsernameLbl) + Pos.Percent(10),
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
                X = Pos.Right(userPasswordInput) + Pos.Percent(10),
                Y = Pos.Top(userPasswordLbl),
            };
            userConfirmPasswordInput = new TextField("")
            {
                X = Pos.Right(userConfirmPasswordLbl) + Pos.Percent(5),
                Y = Pos.Top(userConfirmPasswordLbl),
                Width = Dim.Percent(10),
                Secret = true,
            };
            this.Add(userPasswordLbl, userPasswordInput, userConfirmPasswordLbl, userConfirmPasswordInput);

            Label userFullnameLbl = new Label("Fullname: ")
            {
                X = Pos.Left(userUsernameLbl),
                Y = Pos.Bottom(userPasswordLbl) + Pos.Percent(10),
            };
            userFullnameInput = new TextField("")
            {
                X = Pos.Right(userFullnameLbl) + Pos.Percent(5),
                Y = Pos.Top(userFullnameLbl),
                Width = Dim.Percent(25),
            };
            this.Add(userFullnameLbl, userFullnameInput);

            Button registerBtn = new Button("Register")
            {
                X = Pos.Left(userUsernameLbl),
                Y = Pos.Bottom(userFullnameLbl) + Pos.Percent(10),
                AutoSize = true,
            };
            registerBtn.Clicked += OnRegister;
            this.Add(registerBtn);

            Button backBtn = new Button("Back");
            backBtn.Clicked += OnBack;
            this.AddButton(backBtn);
        }

        public void SetRepository(UserRepository userRepository)
        {
            this.userRepository = userRepository;

            this.authentication = new Authentication(userRepository);
        }

        private void OnRegister()
        {
            if (!CheckConfirmationPassword())
            {
                MessageBox.ErrorQuery("Register user", "Password mismatch. Please re-check", "Ok");
            }
            else
            {
                User registerUser = new User()
                {
                    username = userUsernameInput.Text.ToString(),
                    password = userPasswordInput.Text.ToString(),
                    fullname = userFullnameInput.Text.ToString(),
                    createdAt = DateTime.Now,
                };

                if (authentication.Register(registerUser))
                {
                    MessageBox.Query("Register user", "User successfully created", "Ok");
                }
                else
                {
                    MessageBox.Query("Register user", "User can't be created. Please choose another username", "Ok");
                }
            }
        }

        private void OnBack()
        {
            Application.RequestStop();
        }

        private bool CheckConfirmationPassword()
        {
            if (userPasswordInput.Text == userConfirmPasswordInput.Text)
            {
                return true;
            }

            return false;
        }
    }
}