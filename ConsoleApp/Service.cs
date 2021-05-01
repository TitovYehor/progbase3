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
    }
}