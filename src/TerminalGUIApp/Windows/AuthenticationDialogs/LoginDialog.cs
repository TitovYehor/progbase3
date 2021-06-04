using Terminal.Gui;

using ProcessData;

namespace TerminalGUIApp.Windows.AuthenticationDialogs
{
    public class LoginDialog : Dialog
    {
        private User user;

        public bool logged;

        private UserRepository userRepository;
        private Authentication authentication;

        private TextField usernameInput;
        private TextField passwordInput;


        public LoginDialog()
        {
            this.Title = "Login";

            Label usernameLbl = new Label("Username: ")
            {
                X = Pos.Percent(10),
                Y = Pos.Percent(10),
            };
            usernameInput = new TextField("")
            {
                X = Pos.Right(usernameLbl) + Pos.Percent(5),
                Y = Pos.Top(usernameLbl),
                Width = Dim.Percent(20),
            };
            this.Add(usernameLbl, usernameInput);

            Label passwordLbl = new Label("Password: ")
            {
                X = Pos.Left(usernameLbl),
                Y = Pos.Bottom(usernameLbl) + Pos.Percent(10),
            };
            passwordInput = new TextField("")
            {
                X = Pos.Right(passwordLbl) + Pos.Percent(5),
                Y = Pos.Top(passwordLbl),
                Width = Dim.Percent(15),
                Secret = true,
            };
            this.Add(passwordLbl, passwordInput);

            Button loginBtn = new Button("Login");
            loginBtn.Clicked += OnLogin;
            this.AddButton(loginBtn);

            Button backBtn = new Button("Back");
            backBtn.Clicked += OnCancel;
            this.AddButton(backBtn);
        }

        public void SetRepository(UserRepository userRepository)
        {
            this.userRepository = userRepository;
            this.authentication = new Authentication(userRepository);
        }

        public User GetUser
        {
            get
            {
                return user;
            }
        }
    
        private void OnLogin()
        {
            user = authentication.Login(usernameInput.Text.ToString(), passwordInput.Text.ToString());

            if (user == null)
            {
                MessageBox.ErrorQuery("Login user", "Login failed. Re-check username and password", "Ok");
            }
            else
            {
                MessageBox.Query("Login user", "Successfully logged in", "Ok");

                logged = true;

                Application.RequestStop();
            }
        }

        private void OnCancel()
        {
            logged = false;

            Application.RequestStop();
        }
    }
}