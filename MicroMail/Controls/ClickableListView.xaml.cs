using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MicroMail.Controls
{
    public delegate void ListViewEventHandler(object sender, EventArgs args);

    /// <summary>
    /// Interaction logic for ClickableListBox.xaml
    /// </summary>
    public partial class ClickableListView
    {
        public event ListViewEventHandler ItemMouseDown;

        public ClickableListView()
        {
            InitializeComponent();
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            var listItem = element as ListViewItem;
            if (listItem != null)
            {
                listItem.PreviewMouseLeftButtonDown -= ItemOnMouseLeftButtonDown;
            }
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            var listItem = element as ListViewItem;
            if (listItem != null)
            {
                listItem.PreviewMouseLeftButtonDown += ItemOnMouseLeftButtonDown;
            }
            base.PrepareContainerForItemOverride(element, item);
        }

        private void ItemOnMouseLeftButtonDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            var item = sender as ListViewItem;

            if (item != null)
            {
                ItemMouseDown(item.Content, mouseButtonEventArgs);
            }
        }
    }
}
