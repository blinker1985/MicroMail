using System;
using System.Globalization;
using System.Security.Authentication;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using MicroMail.Infrastructure;
using MicroMail.Infrastructure.Helpers;
using MicroMail.Models;

namespace MicroMail.Windows
{
    /// <summary>
    /// Interaction logic for AccountDialog.xaml
    /// </summary>
    public partial class AccountDialog
    {
        private OutgoingMailServiceResolver _serviceResolver;

        public AccountDialog(OutgoingMailServiceResolver serviceResolver)
        {
            _serviceResolver = serviceResolver;
            Account = new Account();
            InitializeComponent();
            Title = "Add new account";
        }

        public AccountDialog(Account account, OutgoingMailServiceResolver serviceResolver)
        {
            Account = account ?? new Account();
            _serviceResolver = serviceResolver;
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
            SecurityType.Text = ToSecureTypeString(Account.SslProtocol);
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
            Account.SslProtocol = ToSslProtocol(SecurityType.Text);
            Account.SaveEmailsLocally = SaveEmailCheckbox.IsChecked.GetValueOrDefault();

            IsSaved = true;

            var service = _serviceResolver.Resolve(Account.ProtocolType);

            if (service.Test(Account))
            {
                Close();
            }
            else
            {
                MessageBox.Show("Could not not connect using the specified settings.");
            }
        }

        private ProtocolTypeEnum GetSelectedProtocol()
        {
            return Pop3ProtocolRadiobutton.IsChecked.GetValueOrDefault() 
                ? ProtocolTypeEnum.Pop3 
                : ProtocolTypeEnum.Imap;
        }

        private SslProtocols ToSslProtocol(string text)
        {
            switch (text)
            {
                case "SSL":
                    return SslProtocols.Ssl2;
                case "TLS":
                    return SslProtocols.Tls;
                case "None":
                    return SslProtocols.None;
            }

            return SslProtocols.Default;
        }

        private string ToSecureTypeString(SslProtocols protocol)
        {
            switch (protocol)
            {
                case SslProtocols.None:
                    return "None";
                case SslProtocols.Ssl2:
                    return "SSL";
                case SslProtocols.Tls:
                case SslProtocols.Tls11:
                case SslProtocols.Tls12:
                    return "TLS";
            }
            return "Auto";
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
