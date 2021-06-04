using Terminal.Gui;

using ProcessData;


namespace TerminalGUIApp.Windows.ExportAndImportWindows
{
    public class ExportDataDialog : Dialog
    {
        private TextField exportSearchInput;
        private string directoryPathLbl = "../data/exports/postsAndComments";
        private TextField chooseNameInput;

        private PostRepository postsRepository;
        private CommentRepository commentsRepository;


        public ExportDataDialog()
        {
            Label searchLbl = new Label("Enter value to filter posts: ")
            {
                X = Pos.Percent(10),
                Y = Pos.Percent(10),
            };
            exportSearchInput = new TextField("")
            {
                X = Pos.Right(searchLbl) + Pos.Percent(1),
                Y = Pos.Top(searchLbl),
                Width = Dim.Percent(25),
            };
            this.Add(searchLbl, exportSearchInput);

            Label choosePathLbl = new Label("Choose directory path to export: ")
            {
                X = Pos.Left(searchLbl),
                Y = Pos.Percent(20),
            };
            Button choosePathBtn = new Button("Choosing directory...")
            {
                X = Pos.Right(choosePathLbl) + Pos.Percent(1),
                Y = Pos.Top(choosePathLbl),
                AutoSize = true,
            };
            choosePathBtn.Clicked += OnSelectDirectory;
            this.Add(choosePathLbl, choosePathBtn);

            Label chooseNameLbl = new Label("Enter file name to export: ")
            {
                X = Pos.Left(searchLbl),
                Y = Pos.Percent(30),
            };
            chooseNameInput = new TextField("")
            {
                X = Pos.Right(chooseNameLbl) + Pos.Percent(1),
                Y = Pos.Top(chooseNameLbl),
                Width = Dim.Percent(25),
            };
            this.Add(chooseNameLbl, chooseNameInput);

            Button exportBtn = new Button("Export")
            {
                X = Pos.Left(searchLbl),
                Y = Pos.Percent(40),
                AutoSize = true,
            };
            exportBtn.Clicked += OnExport;
            this.Add(exportBtn);

            Button backBtn = new Button("Back")
            {
                X = Pos.Center(),
                Y = Pos.Percent(80),
                AutoSize = true,
            };
            backBtn.Clicked += Application.RequestStop;
            this.Add(backBtn);
        }

        public void SetRepositories(PostRepository postsRepository, CommentRepository commentsRepository)
        {
            this.postsRepository = postsRepository;
            this.commentsRepository = commentsRepository;
        }

        private void OnSelectDirectory()
        {
            OpenDialog dialog = new OpenDialog("Choose directory to export", "Choose?");
            dialog.CanChooseDirectories = true;
            dialog.CanChooseFiles = false;
            dialog.DirectoryPath = directoryPathLbl;

            Application.Run(dialog);

            if (!dialog.Canceled)
            {
                NStack.ustring dirPath = dialog.FilePath;
                directoryPathLbl = dirPath.ToString();
            }
        }

        private void OnExport()
        {
            if (ExportAndImportData.ExportPostsWithComments(exportSearchInput.Text.ToString(),
            directoryPathLbl, chooseNameInput.Text.ToString(), postsRepository, commentsRepository))
            {
                MessageBox.Query("Export data", "Data successfuly exported", "Ok");
            }
            else
            {
                MessageBox.Query("Export data", "Data not exported. Сheck the correctness of the entered data", "Ok");
            }
        }
    }
}