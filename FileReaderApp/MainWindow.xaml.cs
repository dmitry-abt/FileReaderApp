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

        private async void ProcessButton_Click(object sender, RoutedEventArgs e)
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

            ProcessButton.IsEnabled = false;
            ProgressBar.Value = 0;

            try
            {
                await ProcessFilesAsync(inputFilePaths, outputFolderPath, minLength, RemoveShortWordsCheckBox.IsChecked == true, RemovePunctuationCheckBox.IsChecked == true);

                LogTextBox.AppendText("All files processed.\n\n");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                ProcessButton.IsEnabled = true;
            }
        }

        private async Task ProcessFilesAsync(string[] inputFilePaths, string outputFolderPath, int minLength, bool removeShortWords, bool removePunctuation)
        {
            int totalFiles = inputFilePaths.Length;
            int processedFiles = 0;

            foreach (string inputFilePath in inputFilePaths)
            {
                string fileName = Path.GetFileName(inputFilePath);
                string outputFilePath = Path.Combine(outputFolderPath, fileName);

                try
                {
                    await Task.Run(() => ProcessFile(inputFilePath, outputFilePath, minLength, removeShortWords, removePunctuation));

                    processedFiles++;
                    int progress = (int)((double)processedFiles / totalFiles * 100);
                    Dispatcher.Invoke(() => ProgressBar.Value = progress);

                    LogTextBox.AppendText($"Processed: {fileName}\n");
                }
                catch (Exception ex)
                {
                    LogTextBox.AppendText($"Failed to process {fileName}: {ex.Message}\n");
                }
            }
        }

        private void ProcessFile(string inputFilePath, string outputFilePath, int minLength, bool removeShortWords, bool removePunctuation)
        {
            string fileContent = File.ReadAllText(inputFilePath);
            string processedText = ProcessText(fileContent, minLength, removeShortWords, removePunctuation);

            File.WriteAllText(outputFilePath, processedText);
        }

        private string ProcessText(string text, int minLength, bool removeShortWords, bool removePunctuation)
        {
            if (removePunctuation)
            {
                text = RemovePunctuation(text);
            }

            if (removeShortWords)
            {
                text = RemoveShortWords(text, minLength);
            }

            return text;
        }

        private string RemovePunctuation(string text)
        {
            return Regex.Replace(text, @"[\p{P}]", "");
        }

        private string RemoveShortWords(string text, int minLength)
        {
            // Regular expression for searching words
            string pattern = @"\b\w+\b";
            return Regex.Replace(text, pattern, match =>
                match.Value.Length >= minLength ? match.Value : string.Empty);
        }
    }
}