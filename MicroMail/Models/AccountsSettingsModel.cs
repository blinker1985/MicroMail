using System.Collections.Specialized;
using MicroMail.Infrastructure;
using MicroMail.Infrastructure.Helpers;

namespace MicroMail.Models
{
    public delegate void ChangedHandler();

    public class AccountsSettingsModel : SettingsModeBase
    {
        public AccountsSettingsModel()
        {
            Accounts = new AsyncObservableCollection<Account>();
            Load();
        }

        public AsyncObservableCollection<Account> Accounts { get; private set; }

        private void Load()
        {
            if (AppSettings.Accounts == null) return;

            foreach (var str in AppSettings.Accounts)
            {
                Accounts.Add(AccountHelper.Decode(str));
            }
        }

        protected override void ProcessSave()
        {
            AppSettings.Accounts = new StringCollection();
            foreach (var account in Accounts)
            {
                AppSettings.Accounts.Add(AccountHelper.Encode(account));
            }

            base.ProcessSave();
        }

    }
    
}
