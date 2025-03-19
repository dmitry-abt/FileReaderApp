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
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                InputFilePathTextBox.Text = openFileDialog.FileName;
            }
        }

        private void BrowseOutputFileButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
            {
                OutputFilePathTextBox.Text = saveFileDialog.FileName;
            }
        }

        private void ProcessButton_Click(object sender, RoutedEventArgs e)
        {
            string inputFilePath = InputFilePathTextBox.Text;
            string outputFilePath = OutputFilePathTextBox.Text;

            if (string.IsNullOrEmpty(inputFilePath) || !File.Exists(inputFilePath))
            {
                MessageBox.Show("Please select a valid input file", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(outputFilePath))
            {
                MessageBox.Show("Please specify an output file", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(MinWordLengthTextBox.Text, out int minLength))
            {
                MessageBox.Show("Please enter a valid number for minimum word length", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string fileContent = File.ReadAllText(inputFilePath);
                string processedText = RemoveShortWords(fileContent, minLength);

                File.WriteAllText(outputFilePath, processedText);

                LogTextBox.AppendText($"File processed successfully: {outputFilePath}\n");
            }
            catch (Exception ex)
            {
                LogTextBox.AppendText($"An error occurred: {ex.Message}: {outputFilePath}\n");

                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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