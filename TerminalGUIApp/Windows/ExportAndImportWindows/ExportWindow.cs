using Terminal.Gui;

using ProcessData;


namespace TerminalGUIApp.Windows.ExportAndImportWindows
{
    public class ExportWindow : Window
    {
        private TextField exportSearchInput;
        private Label directoryPathLbl;
        private TextField chooseNameInput;

        private PostRepository postsRepository;


        public ExportWindow()
        {
            directoryPathLbl = new Label("../data/exports");

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
        }

        public void SetRepositories(PostRepository postsRepository)
        {
            this.postsRepository = postsRepository;
        }

        private void OnSelectDirectory()
        {
            OpenDialog dialog = new OpenDialog("Choose directory to export", "Choose?");
            dialog.CanChooseDirectories = true;
            dialog.CanChooseFiles = false;
            dialog.DirectoryPath = directoryPathLbl.Text;

            Application.Run(dialog);

            if (!dialog.Canceled)
            {
                NStack.ustring dirPath = dialog.FilePath;
                directoryPathLbl.Text = dirPath;
            }
        }

        private void OnExport()
        {
            if (ExportAndImportData.ExportPostsWithComments(exportSearchInput.Text.ToString(),
            directoryPathLbl.Text.ToString(), chooseNameInput.Text.ToString(), postsRepository))
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