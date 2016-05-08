using System.Windows;

namespace ZuegerAdressbook.Service
{
    public class MessageDialogService : IMessageDialogService
    {
        public bool OpenConfirmationDialog(string title, string text)
        {
            var result = MessageBox.Show(text, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }

        public string OpenFileDialog()
        {
            string filename = null;

            Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();

            var result = fileDialog.ShowDialog();
            if (result == true)
            {
                // Open document 
                filename = fileDialog.FileName;
            }

            return filename;
        }

        public void OpenInformationDialog(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void OpenErrorDialog(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
