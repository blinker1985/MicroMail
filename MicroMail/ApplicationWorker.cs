using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Timers;
using System.Windows;
using MicroMail.Infrastructure;
using MicroMail.Infrastructure.Messaging;
using MicroMail.Infrastructure.Messaging.Events;
using MicroMail.Models;
using MicroMail.Services;
using MicroMail.Windows;
using Ninject;
using Ninject.Syntax;

namespace MicroMail
{
    class ApplicationWorker
    {
        private Application _application;

        private Tray _tray;
        private readonly IDictionary<string, IFetchMailService> _servicesPool = new Dictionary<string, IFetchMailService>();
        private readonly Timer _timer = new Timer();

        private readonly AccountsSettingsModel _accountsSettings;
        private readonly EventBus _eventBus;
        private readonly IResolutionRoot _injector;
        private readonly AsyncObservableCollection<EmailGroupModel> _emailGroupList;

        [Inject]
        public ApplicationWorker(AccountsSettingsModel accountsSettings, EventBus eventBus, IResolutionRoot injector, AsyncObservableCollection<EmailGroupModel> emailGroupList)
        {
            _accountsSettings = accountsSettings;
            _eventBus = eventBus;
            _injector = injector;
            _emailGroupList = emailGroupList;
        }

        public void Start(Application application)
        {
            _application = application;
            _accountsSettings.Changed += AccountsSettingsChangedHandler;
            _timer.Elapsed += TimerElapsedHandler;
            _eventBus.Subscribe(PlainServiceEvents.NewMailFetched, ServiceMailFetchedHandler);
            _eventBus.Subscribe<FetchMailBodyEvent>(FetchMailBodyCallback);

            InitTray();
            InitServices();
            ResetTimer();
        }

        private void TimerElapsedHandler(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            CheckMailInAllServices();
        }

        private void CheckMailInAllServices()
        {
            foreach (var service in _servicesPool)
            {
                service.Value.CheckMail();
            }
        }

        private void AccountsSettingsChangedHandler()
        {
            // TODO: (re-)init only the changed/added accounts instead of all.

            _timer.Stop();

            InitServices();
            ResetTimer();
        }

        private void InitTray()
        {
            _tray = new Tray();
            _tray.Create();
            _tray.About += TrayAboutHandler;
            _tray.CheckMail += TrayCheckMailHandler;
            _tray.Exit += TrayExitHandler;
            _tray.Settings += TraySettingsHandler;
            _tray.Click += TrayClickHandler;
            _tray.BallonClick += TrayBallonClickHandler;
        }

        private void InitServices()
        {
            foreach (var service in _servicesPool)
            {
                service.Value.Stop();
            }

            _servicesPool.Clear();

            foreach (var account in _accountsSettings.Accounts)
            {
                StartAccount(account);
            }
        }

        public void ResetTimer()
        {
            _timer.Interval = _accountsSettings.RefreshTime * 1000;
            _timer.Start();
        }

        private void TraySettingsHandler()
        {
            var window = new SettingsWindow(_accountsSettings);
            window.Show();
        }

        private void TrayExitHandler()
        {
            _application.Exit += ApplicationOnExit; 
            _application.Shutdown();
        }

        private void ApplicationOnExit(object sender, ExitEventArgs exitEventArgs)
        {
            _tray.Dispose();
        }

        private void TrayAboutHandler()
        {
           _eventBus.Trigger(PlainWindowEvents.ShowAboutWindow);
        }

        private void TrayCheckMailHandler()
        {
            CheckMailInAllServices();
            ResetTimer();
        }

        private void TrayClickHandler()
        {
            _eventBus.Trigger(PlainWindowEvents.ShowMailListWindow);
        }

        private void TrayBallonClickHandler()
        {
            _eventBus.Trigger(PlainWindowEvents.ShowMailListWindow);
        }

        private void StartAccount(Account account)
        {
            var service = account != null ? _injector.Get(account.ServiceType) as IFetchMailService : null;

            if (service == null) return;
            _emailGroupList.Add(service.EmailGroup);
            _servicesPool.Add(account.Id, service);
            service.Start(account);
        }

        private void ServiceMailFetchedHandler(object o)
        {
            var emails = _servicesPool.SelectMany(m => m.Value.EmailGroup.EmailList.Where(e => !e.IsRead)).ToArray();
            var count = emails.Count();

            if (count == 0) return;

            if (count > 1)
            {
                _tray.ShowNotification("Mail", "You have new mail. Click here to see", 10);
            }
            else
            {
                var newMail = emails.FirstOrDefault();
                if (newMail != null)
                {
                    _tray.ShowNotification("Mail", newMail.From + "\n" + newMail.Subject, 10);
                }
            }

            _tray.ShowText(emails.Count().ToString(CultureInfo.InvariantCulture));
        }

        private void FetchMailBodyCallback(FetchMailBodyEvent message)
        {
            var email = message.Email;
            var service = email != null && _servicesPool.ContainsKey(message.Email.ServiceId) ? _servicesPool[message.Email.ServiceId] : null;

            if (service == null) return;

            service.FetchMailBody(message.Email);
        }
    }
}
