namespace ZuegerAdressbook.Service
{
    public interface IMessageDialogService
    {
        bool OpenConfirmationDialog(string title, string text);

        string OpenFileDialog();
    }
}