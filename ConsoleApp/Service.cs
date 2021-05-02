namespace ConsoleApp
{
    public class Service
    {
        private UserRepository userRepository;

        private PostRepository postRepository;

        private CommentRepository commentRepository;

        public Service(UserRepository userRepository, PostRepository postRepository, CommentRepository commentRepository)
        {
            this.userRepository = userRepository;

            this.postRepository = postRepository;

            this.commentRepository = commentRepository;
        }        

        public int[] GetAllUsersId()
        {
            User[] users = userRepository.GetAllUsers();

            int[] ids = new int[users.Length];

            for (int i = 0; i < users.Length; i++)
            {
                ids[i] = users[i].id;
            }

            return ids;
        }

        public int[] GetAllPostsId()
        {
            Post[] posts = postRepository.GetAllPosts();

            int[] ids = new int[posts.Length];

            for (int i = 0; i < posts.Length; i++)
            {
                ids[i] = posts[i].id;
            }

            return ids;
        }
    }
}