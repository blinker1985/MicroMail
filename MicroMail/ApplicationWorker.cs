using System;
using System.Collections.Generic;
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
        private bool _newMailInList;

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
            _eventBus.Subscribe<ServiceStatusEvent>(ServiceStateCallback);

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
            _tray.ShowRefreshingIcon();
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
            OpenMailList();
        }

        private void TrayBallonClickHandler()
        {
            OpenMailList();
        }

        private void OpenMailList()
        {
            _eventBus.Trigger(PlainWindowEvents.ShowMailListWindow);
            _newMailInList = false;
            ShowIdleIcon();
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

            _newMailInList = true;
            ShowIdleIcon();

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
        }

        private void FetchMailBodyCallback(FetchMailBodyEvent e)
        {
            this.Debug("Fetch Body");

            var email = e.Email;
            var service = email != null && _servicesPool.ContainsKey(e.Email.ServiceId) ? _servicesPool[e.Email.ServiceId] : null;

            if (service == null) return;

            service.FetchMailBody(e.Email);
        }

        private void ServiceStateCallback(ServiceStatusEvent e)
        {
            switch (e.Status)
            {
                case ServiceStatusEnum.Disconnected:
                case ServiceStatusEnum.FailedRead:
                    _tray.ShowErrorIcon("Disconnected.");
                    break;

                case ServiceStatusEnum.CheckingMail:
                case ServiceStatusEnum.Logging:
                case ServiceStatusEnum.SyncFolder:
                    _tray.ShowRefreshingIcon();
                    break;

                default:
                    ShowIdleIcon();
                    break;
            }
        }

        private void ShowIdleIcon()
        {
            if (_servicesPool.Any(m => m.Value.CurrentStatus != ServiceStatusEnum.Idle)) return;

            if (_newMailInList)
            {
                _tray.ShowUnreadMailIcon();
            }
            else
            {
                _tray.ShowNormalIcon();
            }
        }

    }
}
