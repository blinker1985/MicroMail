using System.Windows;

namespace MicroMail.Models
{
    public class ApplicationSettingsModel :SettingsModeBase
    {
        public int RefreshTime
        {
            get { return AppSettings.RefreshTime; }
            set { AppSettings.RefreshTime = value; }
        }

        public bool FetchNewMail
        {
            get { return AppSettings.FetchNewMails; }
            set { AppSettings.FetchNewMails = value; }
        }

        public Rect MailListRect
        {
            get { return AppSettings.MailListRect; }
            set { AppSettings.MailListRect = value; }
        }

    }
}
