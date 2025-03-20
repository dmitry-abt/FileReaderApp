using Microsoft.Win32;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

namespace FileReaderApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BrowseInputFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                InputFilePathTextBox.Text = string.Join(";", openFileDialog.FileNames);
            }
        }

        private void BrowseOutputFolderButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog dialog = new();
            dialog.Multiselect = false;
            dialog.Title = "Select a folder";

            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                // Get the selected folder
                string fullPathToFolder = dialog.FolderName;
                string folderNameOnly = dialog.SafeFolderName;

                OutputFolderPathTextBox.Text = dialog.FolderName;
            }
        }

        private void ProcessButton_Click(object sender, RoutedEventArgs e)
        {
            string[] inputFilePaths = InputFilePathTextBox.Text.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            string outputFolderPath = OutputFolderPathTextBox.Text;

            if (inputFilePaths.Length == 0)
            {
                MessageBox.Show("Please select at least one input file", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(outputFolderPath) || !Directory.Exists(outputFolderPath))
            {
                MessageBox.Show("Please specify a valid output folder", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(MinWordLengthTextBox.Text, out int minLength))
            {
                MessageBox.Show("Please enter a valid number for minimum word length", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                ProcessFiles(inputFilePaths, outputFolderPath, minLength);

                LogTextBox.AppendText("All files processed.\n");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ProcessFiles(string[] inputFilePaths, string outputFolderPath, int minLength)
        {
            foreach (string inputFilePath in inputFilePaths)
            {
                string fileName = Path.GetFileName(inputFilePath);
                string outputFilePath = Path.Combine(outputFolderPath, fileName);

                try
                {
                    if (File.Exists(inputFilePath))
                    {
                        ProcessFile(inputFilePath, outputFilePath, minLength);

                        LogTextBox.AppendText($"Processed: {fileName}\n");
                    }
                }
                catch (Exception ex)
                {
                    LogTextBox.AppendText($"Failed to process {fileName}: {ex.Message}\n");
                }
            }
        }

        private void ProcessFile(string inputFilePath, string outputFilePath, int minLength)
        {
            string fileContent = File.ReadAllText(inputFilePath);
            string processedText = RemoveShortWords(fileContent, minLength);

            File.WriteAllText(outputFilePath, processedText);
        }

        private static string RemoveShortWords(string text, int minLength)
        {
            // Regular expression for searching words
            string pattern = @"\b\w+\b";
            return Regex.Replace(text, pattern, match =>
                match.Value.Length >= minLength ? match.Value : string.Empty);
        }
    }
}