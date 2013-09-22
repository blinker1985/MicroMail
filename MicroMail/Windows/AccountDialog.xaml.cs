using System.Windows;
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
            Account.EncryptedPassword = AccountHelper.EncryptPassword(Pass.Password);
            IsSaved = true;
            Close();
        }

        private void Cancel_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
