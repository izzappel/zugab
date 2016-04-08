using System.Windows;
using System.Windows.Controls;

namespace ZuegerAdressbook.View.Controls
{
    public class QuickAccessListBox : FrameworkElement
    {
        static QuickAccessListBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(QuickAccessListBox), new FrameworkPropertyMetadata(typeof(QuickAccessListBox)));
        }
    }
}
