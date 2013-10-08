using System.Threading;
using MicroMail.Properties;

namespace MicroMail.Models
{
    public class SettingsModeBase
    {
        public event ChangedHandler Changed;

        internal Settings AppSettings {
            get { return Settings.Default; }
        }

        public void Save()
        {
            var thread = new Thread(ProcessSave);
            thread.Start();
        }

        protected virtual void ProcessSave()
        {
            AppSettings.Save();

            if (Changed != null)
            {
                Changed();
            }
        }
    }
}
