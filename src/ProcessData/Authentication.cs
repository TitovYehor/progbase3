

namespace ProcessData
{
    public class Authentication
    {
        private UserRepository userRepository;

        public Authentication(UserRepository userRepository)
        {
            this.userRepository = userRepository;
        }


        public bool Register(User user)
        {
            if (userRepository.UserExists(user.username))
            {
                return false;
            }
            else
            {
                user.password = HashModule.Hash(user.password);

                userRepository.Insert(user);
            }

            return true;
        }    

        public User Login(string username, string password)
        {
            if (userRepository.UserExists(username))
            {
                User databaseUser = userRepository.GetByUsername(username);

                string hashPassword = HashModule.Hash(password);

                if (hashPassword == databaseUser.password)
                {
                    return databaseUser;
                }
            }

            return null;
        }    
    }
}