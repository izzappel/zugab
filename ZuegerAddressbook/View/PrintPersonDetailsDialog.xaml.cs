using System.Windows;
using ZuegerAdressbook.ViewModels;

namespace ZuegerAdressbook.View
{
    /// <summary>
    /// Interaction logic for PrintPersonDetailsDialog.xaml
    /// </summary>
    public partial class PrintPersonDetailsDialog : Window
    {
        private readonly PrintPersonDetailsViewModel _viewModel;

        public PrintPersonDetailsDialog()
        {
            InitializeComponent();

            _viewModel = IocKernel.Get<PrintPersonDetailsViewModel>();
            DataContext = _viewModel;
        }
    }
}
