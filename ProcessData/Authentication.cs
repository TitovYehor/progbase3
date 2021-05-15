using System.Security.Cryptography;
using System.Text;

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
                user.password = GetHashPassword(user.password);

                userRepository.Insert(user);
            }

            return true;
        }    

        public User Login(string username, string password)
        {
            if (userRepository.UserExists(username))
            {
                User databaseUser = userRepository.GetByUsername(username);

                string hashPassword = GetHashPassword(password);

                if (hashPassword == databaseUser.password)
                {
                    return databaseUser;
                }
            }

            return null;
        }


        private string GetHashPassword(string password)
        {
            SHA256 sha256Hash = SHA256.Create();

            string hashPassword = GetHash(sha256Hash, password);

            sha256Hash.Dispose();

            return hashPassword;
        }

        private string GetHash(HashAlgorithm hashAlgorithm, string input)
        { 
            // Convert the input string to a byte array and compute the hash.
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
 
            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();
 
            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
 
            // Return the hexadecimal string.
            return sBuilder.ToString();
        }    
    }
}