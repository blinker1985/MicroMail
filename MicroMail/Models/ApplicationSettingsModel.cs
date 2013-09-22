using System.Windows;

namespace MicroMail.Models
{
    public class ApplicationSettingsModel :SettingsModeBase
    {
        public Rect MailListRect
        {
            get { return AppSettings.MailListRect; }
            set { AppSettings.MailListRect = value; }
        }

    }
}
