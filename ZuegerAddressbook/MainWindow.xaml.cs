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

            _viewModel = IocKernel.Get<MainViewModel>();
            DataContext = _viewModel;
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

        private void BirthdayFilterToggle_OnChecked(object sender, RoutedEventArgs e)
        {
            if (BirthdayFilterToggle.IsChecked.GetValueOrDefault())
            {
                //_viewModel.SortPersonsByBirthday();
            }
            else
            {
                //_viewModel.SortPersonsByName();
            }
        }

        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            ToolBar toolBar = sender as ToolBar;
            var overflowGrid = toolBar.Template.FindName("OverflowGrid", toolBar) as FrameworkElement;
            if (overflowGrid != null)
            {
                overflowGrid.Visibility = Visibility.Collapsed;
            }

            var mainPanelBorder = toolBar.Template.FindName("MainPanelBorder", toolBar) as FrameworkElement;
            if (mainPanelBorder != null)
            {
                mainPanelBorder.Margin = new Thickness(0);
            }
        }
    }
}
