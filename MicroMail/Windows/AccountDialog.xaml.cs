using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using MicroMail.Infrastructure.Helpers;
using MicroMail.Models;

namespace MicroMail.Windows
{
    /// <summary>
    /// Interaction logic for AccountDialog.xaml
    /// </summary>
    public partial class AccountDialog
    {
        public AccountDialog()
        {
            Account = new Account();
            InitializeComponent();
            Title = "Add new account";
        }

        public AccountDialog(Account account)
        {
            Account = account ?? new Account();
            InitializeComponent();
            Loaded += LoadedHandler;
            Title = "Edit account";
        }

        public Account Account { get; private set; }

        public bool IsSaved { get; private set; }

        private void LoadedHandler(object sender, RoutedEventArgs routedEventArgs)
        {
            Loaded -= LoadedHandler;
            Pass.Password = AccountHelper.ToInsecurePassword(Account.SecuredPassword);
        }

        private void Ok_ClickHandler(object sender, RoutedEventArgs e)
        {
            Account.DeleteReadEmails = DeleteEmailCheckbox.IsChecked.GetValueOrDefault();
            Account.EncryptedPassword = AccountHelper.EncryptPassword(Pass.Password);
            Account.Host = Host.Text;
            Account.Login = Login.Text;
            Account.Name = AccountName.Text;
            Account.Port = int.Parse(Port.Text);
            Account.ProtocolType = GetSelectedProtocol();
            Account.SaveEmailsLocally = SaveEmailCheckbox.IsChecked.GetValueOrDefault();

            IsSaved = true;
            Close();
        }

        private ProtocolTypeEnum GetSelectedProtocol()
        {
            return Pop3ProtocolRadiobutton.IsChecked.GetValueOrDefault() 
                ? ProtocolTypeEnum.Pop3 
                : ProtocolTypeEnum.Imap;
        }

        private void Cancel_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Port_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }
    }

    internal class ImapCheckedValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is ProtocolTypeEnum && (ProtocolTypeEnum) value == ProtocolTypeEnum.Imap;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class Pop3CheckedValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is ProtocolTypeEnum && (ProtocolTypeEnum)value == ProtocolTypeEnum.Pop3;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


}
