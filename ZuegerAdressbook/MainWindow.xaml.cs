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
    }
}
