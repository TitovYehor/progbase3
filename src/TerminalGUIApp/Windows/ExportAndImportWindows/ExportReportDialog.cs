using System;
using Terminal.Gui;

using ProcessData;

namespace TerminalGUIApp.Windows.ExportAndImportWindows
{
    public class ExportReportDialog : Dialog
    {
        private PostRepository postRepository;
        private CommentRepository commentRepository;

        private string directoryPath = "../data/exports";
        
        private TextField chooseNameInput;
        private DateField leftDateField;
        private DateField rightDateField;



        public ExportReportDialog()
        {
            Label chooseNameLbl = new Label("Enter file name to export: ")
            {
                X = Pos.Percent(10),
                Y = Pos.Percent(10),
            };
            chooseNameInput = new TextField("")
            {
                X = Pos.Right(chooseNameLbl) + Pos.Percent(1),
                Y = Pos.Top(chooseNameLbl),
                Width = Dim.Percent(25),
            };
            this.Add(chooseNameLbl, chooseNameInput);

            Label choosePathLbl = new Label("Choose directory path to export: ")
            {
                X = Pos.Left(chooseNameLbl),
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

            Label dateLbl = new Label("Enter date to generate graphic: ")
            {
                X = Pos.Left(choosePathLbl),
                Y = Pos.Percent(30),
                AutoSize = true,
            };
            Label firstSeparateLbl = new Label("From ")
            {
                X = Pos.Left(dateLbl),
                Y = Pos.Bottom(dateLbl) + Pos.Percent(5),
                AutoSize = true,
            }; 
            leftDateField = new DateField()
            {
                X = Pos.Right(firstSeparateLbl),
                Y = Pos.Top(firstSeparateLbl),
                AutoSize = true,
            };
            Label secondSeparateLbl = new Label(" To ")
            {
                X = Pos.Right(leftDateField),
                Y = Pos.Top(leftDateField),
                AutoSize = true,
            };
            rightDateField = new DateField()
            {
                X = Pos.Right(secondSeparateLbl),
                Y = Pos.Top(secondSeparateLbl),
                AutoSize = true,
            };
            this.Add(dateLbl, firstSeparateLbl, leftDateField, secondSeparateLbl, rightDateField);

            Button generateBtn = new Button("Generate report")
            {
                X = Pos.Left(dateLbl),
                Y = Pos.Bottom(firstSeparateLbl) + Pos.Percent(10),
                AutoSize = true,
            };
            generateBtn.Clicked += OnGenerateReport;
            this.Add(generateBtn);

            Button backBtn = new Button("Back")
            {
                X = Pos.Center(),
                Y = Pos.Percent(80),
                AutoSize = true,
            };
            backBtn.Clicked += Application.RequestStop;
            this.Add(backBtn);
        }

        public void SetRepositories(PostRepository postRepository, CommentRepository commentRepository)
        {
            this.postRepository = postRepository;

            this.commentRepository = commentRepository;
        }
    
        private void OnSelectDirectory()
        {
            OpenDialog dialog = new OpenDialog("Choose directory to export", "Choose?");
            dialog.CanChooseDirectories = true;
            dialog.CanChooseFiles = false;
            dialog.DirectoryPath = directoryPath;

            Application.Run(dialog);

            if (!dialog.Canceled)
            {
                NStack.ustring dirPath = dialog.FilePath;
                directoryPath = dirPath.ToString();
            }
        }
    
        private void OnGenerateReport()
        {
            if (this.ValidateData() == true)
            {   
                DateTime[] dates = new DateTime[]{leftDateField.Date, rightDateField.Date};

                ExportAndImportData.ExportReport(directoryPath, chooseNameInput.Text.ToString(), 
                                                  dates, postRepository, commentRepository);
            }
        }

        private bool ValidateData()
        {
            bool validated = true;

            if (String.IsNullOrWhiteSpace(this.directoryPath))
            {
                MessageBox.ErrorQuery("Generating graphic", "Graphic path does not correct. Please re-check", "Ok");

                validated = false;
            }
            
            if (String.IsNullOrWhiteSpace(chooseNameInput.Text.ToString()))
            {
                MessageBox.ErrorQuery("Generating graphic", "Graphic name does not correct. Please re-check", "Ok");

                validated = false;
            }

            if (leftDateField.Date >= rightDateField.Date)
            {
                MessageBox.ErrorQuery("Generating graphic", "Time intervals does not correct. Please re-check", "Ok");

                validated = false;
            }

            return validated;
        }
    }
}