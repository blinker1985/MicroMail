using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading;
using MicroMail.Infrastructure.Messaging;
using MicroMail.Infrastructure.Messaging.Events;
using MicroMail.Models;

namespace MicroMail.Services
{
    public abstract class FetchMailServiceBase : IFetchMailService
    {
        private TcpClient _tcpClient;
        private SslStream _ssl;
        private readonly EventBus _eventBus;
        private readonly object _locker = new object();

        protected FetchMailServiceBase(EventBus eventBus)
        {
            _eventBus = eventBus;
            EmailGroup = new EmailGroupModel();
        }

        public Account Account { get; private set; }
        public string AccountId { get; private set; }
        public EmailGroupModel EmailGroup { get; private set; }

        private ServiceStatusEnum _currentStatus;
        public ServiceStatusEnum CurrentStatus
        {
            get { return _currentStatus; }
            private set
            {
                _currentStatus = value;
                TriggetEvent(new ServiceStatusEvent(value));
            }
        }

        public void Init(Account account)
        {
            if (account == null) return;

            EmailGroup.Name = string.Format("{0} ({1})", account.Name, account.Login);
            EmailGroup.AccountId = account.Id;

            Account = account;
            AccountId = account.Id;
        }

        public virtual void Start()
        {
            InitConnection();
            Login();
            CheckMail();
        }

        public virtual void Stop()
        {
            _tcpClient.Close();
        }

        public abstract void Login();
        public abstract void CheckMail();
        public abstract void FetchMailBody(EmailModel email);

        private void InitConnection()
        {
            try
            {
                _tcpClient = new TcpClient(Account.Host, Account.Port);
                _ssl = new SslStream(_tcpClient.GetStream());
                _ssl.AuthenticateAsClient(Account.Host);
            }
            catch
            {
                CurrentStatus = ServiceStatusEnum.Disconnected;
            }
        }

        protected void SendCommand(IServiceCommand request, ServiceStatusEnum status)
        {
            if (request == null) return;

            var thread = new Thread(() =>
            {
                lock (_locker)
                {
                    SendCommandAsync(request, status);
                }
            });
            thread.Start();
        }

        private void SendCommandAsync(IServiceCommand command, ServiceStatusEnum status)
        {
            CurrentStatus = status;
            var newStatus = ServiceStatusEnum.Idle;

            if (_tcpClient == null || !_tcpClient.Connected)
            {
                CurrentStatus = ServiceStatusEnum.Disconnected;
                return;
            }

            command.Execute(_ssl);

            //try
            //{
            //    command.Execute(_ssl);
            //}
            //catch
            //{
            //    newStatus = ServiceStatusEnum.FailedRead;
            //}

            CurrentStatus = newStatus;
        }

        protected void TriggetEvent(object e)
        {
            _eventBus.Trigger(e);
        }

        protected void TriggetEvent(string e)
        {
            _eventBus.Trigger(e);
        }

        protected void AddNewEmails(IEnumerable<EmailModel> emails)
        {
            foreach (var email in emails)
            {
                EmailGroup.EmailList.Add(email);
            }
            TriggetEvent(PlainServiceEvents.NewMailFetched);
        }

    }
}
