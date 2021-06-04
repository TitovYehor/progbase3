using ProcessData;

namespace TerminalGUIApp.Windows.CommentWindows
{
    public class EditCommentDialog : CreateCommentDialog
    {
        public EditCommentDialog()
        {
            this.Title = "Edit comment";
        }

        public void SetComment(Comment comment)
        {
            this.idLbl.Text = comment.id.ToString();
            this.commentContentInput.Text = comment.content;
            this.commentCreatedAtDateField.Text = comment.createdAt.ToShortDateString();
            this.commentUserIdInput.Text = comment.userId.ToString();
            this.commentPostIdInput.Text = comment.postId.ToString();
        }
    }
}