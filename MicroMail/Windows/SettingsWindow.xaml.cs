using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using MicroMail.Models;

namespace MicroMail.Windows
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : ISingularWindow
    {
        public AccountsSettingsModel Model { get; private set; }
        public int Number { get; set; }

        public SettingsWindow(AccountsSettingsModel accountsSettings)
        {
            Model = accountsSettings;
            InitializeComponent();
        }

        public string SingularId {
            get { return "Settings"; }
        }

        private void SaveButtonClickHandler(object sender, RoutedEventArgs e)
        {
            //TODO: try to connect connect with the inputed data to validate
            Model.Save();
            Close();
        }

        private void CancelButtonClickHandler(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AddAccount_ClickHandler(object sender, RoutedEventArgs e)
        {
            var window = new AccountDialog();
            window.Closed += AccountDialog_ClosedHandler;
            window.Show();
        }

        private void EditAccount_ClickHandler(object sender, RoutedEventArgs e)
        {
            var acc = (Account)AccountsList.SelectedItem;
            var window = new AccountDialog(acc);
            window.Show();
        }

        private void AccountDialog_ClosedHandler(object sender, EventArgs eventArgs)
        {
            var window = (AccountDialog) sender;
            if (window == null) return;
            window.Closed -= AccountDialog_ClosedHandler;

            if (window.IsSaved)
            {
                Model.Accounts.Add(window.Account);
            }
        }

        private void DeleteAccount_ClickHandler(object sender, RoutedEventArgs e)
        {
            var acc = (Account) AccountsList.SelectedItem;
            if (acc == null) return;

            var appendix = acc.SaveEmailsLocally ? " (and its locally saved emails)" : "";
            var message = string.Format("Are you sure you want to delete " + acc.Name + " account{0}?", appendix);

            var res = MessageBox.Show(message, "Warning", MessageBoxButton.YesNo);

            if (res == MessageBoxResult.Yes)
            {
                Model.Accounts.Remove(acc);
            }
        }

    }

    class ObjectToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
