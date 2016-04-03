using System.Windows;
using System.Windows.Controls;

using ZuegerAdressbook.ViewModels;

namespace ZuegerAdressbook
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            Style = (Style)FindResource(typeof(Window));

            _viewModel = new MainViewModel();
            DataContext = _viewModel;
        }

        private void BirthdateButton_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new BirthdateDialog();
            dialog.ShowDialog();
        }

        private void AddDocumentButton_OnClick(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();

            var result = fileDialog.ShowDialog();
            if (result == true)
            {
                // Open document 
                string filename = fileDialog.FileName;
            }
        }
    }
}
