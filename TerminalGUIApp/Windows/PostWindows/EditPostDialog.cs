using ProcessData;

namespace TerminalGUIApp.Windows.PostWindows
{
    public class EditPostDialog : CreatePostDialog
    {
        public EditPostDialog()
        {
            this.Title = "Edit post";
        }

        public void SetPost(Post post)
        {
            this.idLbl.Text = post.id.ToString();
            this.postContentInput.Text = post.content;
            this.postCreatedAtDateField.Text = post.createdAt.ToShortDateString();
            this.postUserIdInput.Text = post.userId.ToString();
        }
    }
}