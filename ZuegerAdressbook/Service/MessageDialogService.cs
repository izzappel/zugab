using System.Windows;

namespace ZuegerAdressbook.Service
{
    public class MessageDialogService
    {
        public static bool OpenConfirmationDialog(string title, string text)
        {
            var result = MessageBox.Show(text, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }
    }
}
