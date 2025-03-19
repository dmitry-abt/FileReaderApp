using Microsoft.Win32;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

namespace FileReaderApp
{
    public partial class MainWindow : Window
    {
        private string _fileContent;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                _fileContent = File.ReadAllText(filePath);
                FileContentTextBox.Text = _fileContent;
            }
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_fileContent))
            {
                MessageBox.Show("No file content to filter.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(MinWordLengthTextBox.Text, out int minLength))
            {
                MessageBox.Show("Please enter a valid number for minimum word length.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string filteredText = RemoveShortWords(_fileContent, minLength);
            FileContentTextBox.Text = filteredText;
        }

        public static string RemoveShortWords(string text, int minLength)
        {
            // Regular expression for searching words
            string pattern = @"\b\w+\b";
            return Regex.Replace(text, pattern, match =>
                match.Value.Length >= minLength ? match.Value : string.Empty);
        }
    }
}
