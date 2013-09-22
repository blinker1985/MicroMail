using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using MicroMail.Infrastructure;
using MicroMail.Infrastructure.Messaging;
using MicroMail.Infrastructure.Messaging.Events;
using MicroMail.Models;

namespace MicroMail.Windows
{
    /// <summary>
    /// Interaction logic for MailListWindow.xaml
    /// </summary>
    public partial class MailListWindow : ISingularWindow
    {
        private readonly EventBus _eventBus;
        private readonly ApplicationSettingsModel _appSettings;
        private readonly Rect _defaultWindowRect;
        private const int DefaultWidth = 300;

        public MailListWindow(EventBus eventBus, AsyncObservableCollection<EmailGroupModel> emailGroupList, ApplicationSettingsModel appSettings)
        {
            _eventBus = eventBus;
            _emailGroupListModel = emailGroupList;
            _appSettings = appSettings;

            var screen = Screen.FromHandle(new WindowInteropHelper(this).Handle).WorkingArea;
            _defaultWindowRect = new Rect(screen.Width - DefaultWidth, 0, DefaultWidth, screen.Height);

            InitializeComponent();
            SetWindowRect();
            Closing += OnClosing;
        }

        private readonly AsyncObservableCollection<EmailGroupModel> _emailGroupListModel;

        public AsyncObservableCollection<EmailGroupModel> EmailGroupListModel
        {
            get { return _emailGroupListModel; }
        }

        public string SingularId {
            get { return "MailList"; }
        }

        private void OnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            var windowRect = new Rect(Left, Top, Width, Height);
            if (windowRect != _defaultWindowRect)
            {
                _appSettings.MailListRect = windowRect;
                _appSettings.Save();
            }
            Closing -= OnClosing;
        }

        private void SetWindowRect()
        {
            var settingsRect = _appSettings.MailListRect;
            var rect = settingsRect.Width > 0 && settingsRect.Height > 0
                ? settingsRect 
                : _defaultWindowRect;
            Left = rect.Left;
            Top = rect.Top;
            Width = rect.Width;
            Height = rect.Height;
        }

        private void ClickableListView_OnItemMouseDown(object sender, EventArgs args)
        {
            var email = sender as EmailModel;
            if (email != null)
            {
                _eventBus.Trigger(new ShowMailWindowEvent(email));
            }
        }
    }
}
