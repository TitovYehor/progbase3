using Terminal.Gui;

using ProcessData;


namespace TerminalGUIApp.Windows.ExportAndImportWindows
{
    public class ExportWindow : Window
    {
        private User user;

        // private TextField exportSearchInput;
        // private Label directoryPathLbl;
        // private TextField chooseNameInput;

        private PostRepository postsRepository;
        private CommentRepository commentsRepository;

        private MenuBar mainMenu;


        public ExportWindow()
        {
            // directoryPathLbl = new Label("../data/exports");

            mainMenu = new MenuBar(
                new MenuBarItem[] 
                {
                    new MenuBarItem ("_Export", new MenuItem[]
                    {
                        // new MenuItem("_Export", "", OnExport),
                        new MenuItem("_Quit", "", OnQuit)
                    })
                })
            {
                Width = Dim.Percent(5),
            };
            this.Add(mainMenu);

            Button openExportDataBtn = new Button("Export data")
            {
                X = Pos.Percent(20),
                Y = Pos.Center(),
                AutoSize = true,
            };
            openExportDataBtn.Clicked += OnOpenExportData;
            this.Add(openExportDataBtn);

            Button openExportGraphicBtn = new Button("Export graphic")
            {
                X = Pos.Percent(45),
                Y = Pos.Center(),
                AutoSize = true,
            };
            openExportGraphicBtn.Clicked += OnOpenExportGraphic;
            this.Add(openExportGraphicBtn);

            Button openExportReportBtn = new Button("Export report")
            {
                X = Pos.Percent(70),
                Y = Pos.Top(openExportDataBtn),
                AutoSize = true,
            };
            openExportReportBtn.Clicked += OnOpenExportReport;
            this.Add(openExportReportBtn);

            Button backBtn = new Button("Back")
            {
                X = Pos.Center(),
                Y = Pos.Percent(80),
                AutoSize = true,
            };
            backBtn.Clicked += OnQuit;
            this.Add(backBtn);
        }

        public void SetRepositories(PostRepository postsRepository, CommentRepository commentsRepository)
        {
            this.postsRepository = postsRepository;
            this.commentsRepository = commentsRepository;
        }

        public void SetUser(User user)
        {
            this.user = user;
        }
        
        private void OnOpenExportData()
        {
            ExportDataDialog dialog = new ExportDataDialog();
            dialog.SetRepositories(postsRepository, commentsRepository);
            
            Application.Run(dialog);
        }

        private void OnOpenExportGraphic()
        {
            ExportGraphicDialog dialog = new ExportGraphicDialog();
            dialog.SetRepositories(postsRepository, commentsRepository);
            dialog.SetUser(user);

            Application.Run(dialog);
        }

        private void OnOpenExportReport()
        {
            ExportReportDialog dialog = new ExportReportDialog();
            dialog.SetRepositories(postsRepository, commentsRepository);

            Application.Run(dialog);
        }

        private void OnQuit()
        {
            Application.RequestStop();
        }
    }
}