using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using MicroMail.Infrastructure;
using MicroMail.Infrastructure.Messaging;
using MicroMail.Infrastructure.Messaging.Events;
using MicroMail.Models;

namespace MicroMail.Services
{
    public abstract class FetchMailServiceBase : IFetchMailService
    {

        private TcpClient _tcpClient;
        private SslStream _ssl;
        private readonly Collection<IServiceCommand> _requestsPool = new Collection<IServiceCommand>();
        private readonly EventBus _eventBus;
        private readonly object _locker = new object();

        protected FetchMailServiceBase(EventBus eventBus)
        {
            _eventBus = eventBus;
            EmailGroup = new EmailGroupModel();
            UnreadMessagesIds = new List<string>();
        }

        public Account Account { get; private set; }
        public string ServiceId { get; private set; }
        public List<string> UnreadMessagesIds { get; private set; }
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

        public void Start(Account account)
        {
            if (account == null) return;

            EmailGroup.Name = string.Format("{0} ({1})", account.Name, account.Login);

            Account = account;
            ServiceId = account.Id;

            _tcpClient = new TcpClient(Account.Host, Account.Port);
            _ssl = new SslStream(_tcpClient.GetStream());
            _ssl.AuthenticateAsClient(Account.Host);
            //Init();

            Login();
            CheckMail();
        }

        public void Stop()
        {
            _tcpClient.Close();
            _requestsPool.Clear();
        }

        //private void Init()
        //{
        //    if (!_tcpClient.Connected) return;

        //    _ssl.Flush();
        //    var reader = new StreamReader(_ssl, Encoding.Default);
        //    this.Debug(reader.ReadLine());
        //}

        public abstract void Login();
        public abstract void CheckMail();
        public abstract void FetchMailBody(EmailModel email);
        protected abstract RawObject GetRawObject(string message);

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

            if (!_tcpClient.Connected)
            {
                CurrentStatus = ServiceStatusEnum.Disconnected;
                return;
            }

            try
            {
                _requestsPool.Add(command);
                WriteSsl(command);
                ReadSsl(command.Encoding);
            }
            catch
            {
                newStatus = ServiceStatusEnum.FailedRead;
            }

            CurrentStatus = newStatus;
        }

        private void WriteSsl(IServiceCommand command)
        {
            if (command == null || command.BinMessage == null || command.BinMessage.Length <= 0) return;

            var writer = new StreamWriter(_ssl);
            var mess = Encoding.UTF8.GetString(command.BinMessage);

            writer.WriteLine(mess);
            writer.Flush();
            this.Debug(mess);
        }

        private void ReadSsl(Encoding encoding)
        {
            var message = "";
            RawObject raw = null;
            var reader = new StreamReader(_ssl, encoding);

            while (true)
            {
                var line = reader.ReadLine();

                if (line == null) break;

                message += line + "\r\n";

                raw = GetRawObject(line);
                this.Debug(line);

                if (raw == null) continue;

                raw.Message = message;
                break;
            }


            if (raw == null) return;

            for (var i = _requestsPool.Count - 1; i >= 0; i--)
            {
                var request = _requestsPool[i];
                if (request.Id != raw.Id) continue;

                _requestsPool.Remove(request);
                request.NotifyCallback(raw);
            }
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
