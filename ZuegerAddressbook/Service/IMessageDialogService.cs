using System.Windows.Documents;

namespace ZuegerAdressbook.Service
{
    public interface IMessageDialogService
    {
        bool OpenConfirmationDialog(string title, string text);

        string OpenFileDialog();

        string OpenExcelFileDialog();

        string SaveExcelFileDialog();

        void OpenInformationDialog(string title, string message);

        void OpenErrorDialog(string title, string message);

        void OpenPrintDialog(FlowDocument document, string description);
    }
}