using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using ZuegerAdressbook.Extensions;

namespace ZuegerAdressbook.View.Controls
{
    public partial class BirthdateQuickAccess : UserControl
    {
        public BirthdateQuickAccess()
        {
            InitializeComponent();

            MouseDown += OnMouseDown;
        }

        private int GetMonth(string monthAbbreviation)
        {
            switch (monthAbbreviation)
            {
                case "JAN":
                    return 1;
                case "FEB":
                    return 2;
                case "MAR":
                    return 3;
                case "APR":
                    return 4;
                case "MAI":
                    return 5;
                case "JUN":
                    return 6;
                case "JUL":
                    return 7;
                case "AUG":
                    return 8;
                case "SEP":
                    return 9;
                case "OKT":
                    return 10;
                case "NOV":
                    return 11;
                case "DEZ":
                    return 12;
                default:
                    return 1;
            }
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var textBlock = e.OriginalSource as TextBlock;

            if (textBlock != null && textBlock.Text.IsNullOrEmpty() == false)
            {
                ScrollToMonth(GetMonth(textBlock.Text));
            }
        }

        private void ScrollToMonth(int month)
        {
            if (TargetControl == null || TargetControl.ItemsSource == null)
            {
                return;
            }

            var collectionView = CollectionViewSource.GetDefaultView(TargetControl.ItemsSource);

            if (collectionView == null)
            {
                throw new InvalidOperationException("The TargetControl should use ICollectionView as ItemSource.");
            }

            if (string.IsNullOrEmpty(TargetPropertyPath))
            {
                throw new InvalidOperationException("TargetPropertyPath is not set.");
            }

            var firstAtMonth = collectionView.SourceCollection.Cast<object>().FirstOrDefault(o => o.DynamicAccess<DateTime?>(TargetPropertyPath)?.Month.Equals(month) ?? false);

            if (firstAtMonth != null)
            {
                var scrollViewer = TargetControl.FindChild<ScrollViewer>();
                scrollViewer.ScrollToBottom();
                TargetControl.ScrollIntoView(firstAtMonth);
            }
        }

        public static readonly DependencyProperty TargetControlProperty = DependencyProperty.Register("TargetControl", typeof(ItemsControl), typeof(BirthdateQuickAccess), new UIPropertyMetadata(null));
        public static readonly DependencyProperty TargetPropertyPathProperty = DependencyProperty.Register("TargetPropertyPath", typeof(string), typeof(BirthdateQuickAccess), new PropertyMetadata(string.Empty));

        public ListBox TargetControl
        {
            get { return (ListBox)GetValue(TargetControlProperty); }
            set { SetValue(TargetControlProperty, value); }
        }

        public string TargetPropertyPath
        {
            get { return (string)GetValue(TargetPropertyPathProperty); }
            set { SetValue(TargetPropertyPathProperty, value); }
        }
    }
}
