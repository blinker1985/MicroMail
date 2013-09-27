using System;
using System.Drawing;
using System.Drawing.Text;
using System.Reflection;
using System.Windows.Forms;
using MicroMail.Properties;

namespace MicroMail.Infrastructure
{
    public delegate void TrayEventHandler();

    class Tray : IDisposable
    {
        private NotifyIcon _icon;
        private ContextMenu _menu;

        public event TrayEventHandler About;
        public event TrayEventHandler Settings;
        public event TrayEventHandler Exit;
        public event TrayEventHandler BallonClick;
        public event TrayEventHandler Click;
        public event TrayEventHandler CheckMail;

        public void Create()
        {
            _menu = new ContextMenu();
            _menu.MenuItems.Add("Settings", SettingsClickHandler);
            _menu.MenuItems.Add("About", AboutClickHandler);
            _menu.MenuItems.Add("Check Mail", CheckMailHandler);
            _menu.MenuItems.Add("Exit", ExitClickHandler);

            _icon = new NotifyIcon
            {
                Text = Resources.AppName,
                ContextMenu = _menu,
                Visible = true
            };
            ShowNormalIcon();
            _icon.BalloonTipClicked += IconBalloonClickHandler;
            _icon.Click += IconClickHandler;
        }

        public void ShowNotification(string title, string notification, int timeoutSeconds)
        {
            _icon.BalloonTipText = notification;
            _icon.BalloonTipTitle = title;
            _icon.Visible = true;
            _icon.ShowBalloonTip(timeoutSeconds * 1000);
        }

        public void ShowNormalIcon()
        {
            UpdateIcon("MicroMail.Graphics.trayIconNormal.png");
            _icon.Text = Resources.TrayNoNewMailtext;
        }

        public void ShowRefreshingIcon()
        {
            UpdateIcon("MicroMail.Graphics.trayIconRefresh.png");
            _icon.Text = Resources.TrayCheckingMailText;
        }

        public void ShowUnreadMailIcon()
        {
            UpdateIcon("MicroMail.Graphics.trayIconUnread.png");
            _icon.Text = Resources.TrayUnreadMailText;
        }

        public void ShowErrorIcon(string error)
        {
            UpdateIcon("MicroMail.Graphics.trayIconError.png");
            _icon.Text = error;
        }

        private void UpdateIcon(string source)
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(source);

            if (stream == null) return;

            var bm = new Bitmap(stream);
            _icon.Icon = Icon.FromHandle(bm.GetHicon());
        }

        private void SettingsClickHandler(object sender, EventArgs eventArgs)
        {
            Settings();
        }

        private void AboutClickHandler(object sender, EventArgs eventArgs)
        {
            About();
        }

        private void CheckMailHandler(object sender, EventArgs eventArgs)
        {
            CheckMail();
        }

        private void ExitClickHandler(object sender, EventArgs e)
        {
            Exit();
        }
        
        private void IconBalloonClickHandler(object sender, EventArgs eventArgs)
        {
            BallonClick();
        }

        private void IconClickHandler(object sender, EventArgs eventArgs)
        {
            var mArgs = eventArgs as MouseEventArgs;
            if (mArgs != null && mArgs.Button != MouseButtons.Left) return;
            
            Click();
        }

        public void Dispose()
        {
            _icon.Dispose();
        }
    }
}
