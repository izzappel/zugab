namespace ZuegerAdressbook.Service
{
    public interface IMessageDialogService
    {
        bool OpenConfirmationDialog(string title, string text);

        string OpenFileDialog();

        void OpenInformationDialog(string title, string message);

        void OpenErrorDialog(string title, string message);
    }
}