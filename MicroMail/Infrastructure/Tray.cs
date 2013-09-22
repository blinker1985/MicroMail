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
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MicroMail.Graphics.trayIcon.png");

            if (stream == null) return;
            
            var bm = new Bitmap(stream);
            _icon.Icon = Icon.FromHandle(bm.GetHicon());
            _icon.BalloonTipClicked += IconBalloonClickHandler;
            _icon.Click += IconClickHandler;
        }

        public void ShowText(string text)
        {
            _icon.Icon.Dispose();

            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Remoting.Graphics.trayIcon.png");

            if (stream == null) return;

            var bm = new Bitmap(stream);
            var graphics = Graphics.FromImage(bm);
            graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixel;

            var family = new FontFamily("Arial");
            var font = new Font(family, 18, FontStyle.Regular, GraphicsUnit.Point);

            graphics.DrawString(text, font, new SolidBrush(Color.Azure), 0, 0);
            _icon.Icon = Icon.FromHandle(bm.GetHicon());
        }

        public void ShowNotification(string title, string notification, int timeoutSeconds)
        {
            _icon.BalloonTipText = notification;
            _icon.BalloonTipTitle = title;
            _icon.Visible = true;
            _icon.ShowBalloonTip(timeoutSeconds * 1000);
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
