using Terminal.Gui;

using ProcessData;


namespace TerminalGUIApp.Windows.ExportAndImportWindows
{
    public class ImportWindow : Window
    {
        private UserRepository usersRepository;
        private PostRepository postsRepository;
        private CommentRepository commentsRepository; 

        private Label filePathLbl = new Label("../data/exports");


        public ImportWindow()
        {
            Label choosePathLbl = new Label("Choose directory path to export: ")
            {
                X = Pos.Percent(10),
                Y = Pos.Percent(10),
            };
            Button choosePathBtn = new Button("Choosing file path...")
            {
                X = Pos.Right(choosePathLbl) + Pos.Percent(1),
                Y = Pos.Top(choosePathLbl),
                AutoSize = true,
            };
            choosePathBtn.Clicked += OnSelectDirectory;
            this.Add(choosePathLbl, choosePathBtn);

            Button importBtn = new Button("Import")
            {
                X = Pos.Left(choosePathLbl),
                Y = Pos.Percent(30),
                AutoSize = true,
            };
            importBtn.Clicked += OnImport;
            this.Add(importBtn);
        }

        public void SetRepositories(UserRepository usersRepository, PostRepository postsRepository, CommentRepository commentsRepository)
        {
            this.usersRepository = usersRepository;
            this.postsRepository = postsRepository;
            this.commentsRepository = commentsRepository;
        }

        private void OnSelectDirectory()
        {
            OpenDialog dialog = new OpenDialog("Choose directory to export", "Choose?");
            dialog.CanChooseDirectories = false;
            dialog.CanChooseFiles = true;
            dialog.DirectoryPath = filePathLbl.Text;
            dialog.AllowedFileTypes = new string[]{".zip"};

            Application.Run(dialog);

            if (!dialog.Canceled)
            {
                NStack.ustring filePath = dialog.FilePath;
                filePathLbl.Text = filePath;
            }
        }
    
        private void OnImport()
        {
            if (ExportAndImportData.ImportPostsWithComments(filePathLbl.Text.ToString(), usersRepository, postsRepository, commentsRepository))
            {
                MessageBox.Query("Import data", "Data successfully imported", "Ok");
            }
            else
            {
                MessageBox.Query("Import data", "Data not imported. Сheck the correctness of the entered data", "Ok");
            }
        }
    }
}