using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MicroMail.Infrastructure.Helpers;
using MicroMail.Infrastructure.Messaging;
using MicroMail.Infrastructure.Messaging.Events;
using MicroMail.Models;
using MicroMail.Services.Commands;
using MicroMail.Services.Responses;

namespace MicroMail.Services
{
    public delegate void ImapServiceHandler(ImapService service);

    public class ImapService : IFetchMailService
    {
        private const string ResponsePattern = "^(?<id>{0})\\s(?<status>OK|NO|BAD)";

        //private StreamReader _reader;
        private TcpClient _tcpClient;
        private SslStream _ssl;
        private readonly Collection<IImapCommand> _requestsPool = new Collection<IImapCommand>();
        private readonly EventBus _eventBus;

        public ImapService(EventBus eventBus)
        {
            _eventBus = eventBus;
            EmailGroup = new EmailGroupModel();
            UnreadMessagesIds = new List<string>();
        }

        public Account Account { get; private set; }
        public string ServiceId { get; private set; }
        public List<string> UnreadMessagesIds { get; private set; }
        public EmailGroupModel EmailGroup { get; private set; }

        public void Start(Account account)
        {
            if (account == null) return;

            EmailGroup.Name = string.Format("{0} ({1})", account.Name, account.Login);

            Account = account;
            ServiceId = account.Id;

            _tcpClient = new TcpClient(Account.Host, Account.Port);
            _ssl = new SslStream(_tcpClient.GetStream());
            _ssl.AuthenticateAsClient(Account.Host);
            //_reader = new StreamReader(_ssl);
            Init();
            Login();
            SelectRootFolder();
            CheckMail();
        }

        public void Stop()
        {
            _tcpClient.Close();
            _requestsPool.Clear();
        }

        private void Init()
        {
            if (!_tcpClient.Connected) return;

            _ssl.Flush();
            var reader = new StreamReader(_ssl, Encoding.Default);
            reader.ReadLine();

            _eventBus.Trigger(PlainServiceEvents.Connected);
        }

        private void Login()
        {
            SendCommand(new LoginCommand(Account.Login, AccountHelper.ToInsecurePassword(Account.SecuredPassword), LoginCallback));
        }

        private void LoginCallback(LoginResponse responseBase)
        {
            _eventBus.Trigger(PlainServiceEvents.LoginSuccess);
        }

        private void SelectRootFolder()
        {
            SendCommand(new SelectRootFolderCommand(null));
        }

        public void CheckMail()
        {
            SendCommand(new SearchUnseenCommand(CheckMailResponder));
        }

        private void CheckMailResponder(SearchUnseenResponse request)
        {
            UnreadMessagesIds.AddRange(request.UnseenIds);
            FetchMailHeaders(request.UnseenIds);
        }

        public void FetchMailHeaders(string[] ids)
        {
            var count = ids.Count();
            var fetchedCount = 0;
            foreach (var id in ids)
            {
                SendCommand(new FetchMailHeadersCommand(id, m =>
                    {
                        fetchedCount++;
                        var email = m.Email;

                        if (email == null || EmailGroup.EmailList.Any(e => e.Id == email.Id)) return;

                        email.ServiceId = ServiceId;
                        EmailGroup.EmailList.Add(email);

                        if (fetchedCount == count)
                        {
                            _eventBus.Trigger(PlainServiceEvents.NewMailFetched);
                        }
                    }));
            }
        }

        public void FetchMailBody(EmailModel email)
        {
            SendCommand(new FetchMailBodyCommand(email, null));
        }

        private void SendCommand(IImapCommand request)
        {
            if (request == null) return;

            try
            {
                if (request.BinMessage.Length > 0)
                {
                    _requestsPool.Add(request);
                    if (_tcpClient.Connected)
                    {
                        _ssl.Write(request.BinMessage, 0, request.BinMessage.Length);
                    }
                    else
                    {
                        _eventBus.Trigger(PlainServiceEvents.Disconnected);
                    }
                }

                _ssl.Flush();
                ReadSsl(request.Encoding);
            }
            catch
            {
                _eventBus.Trigger(PlainServiceEvents.ServiceFailed);
            }
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

                raw = GetRawObject(line);
                if (raw != null)
                {
                    raw.Message = message;
                    break;
                }

                message += line + "\r\n";
            }

            
            if (raw == null) return;

            foreach (var request in _requestsPool.Where(request => raw.Id == request.Id))
            {
                _requestsPool.Remove(request);
                request.NotifyCallback(raw);
            }
        }

        private RawObject GetRawObject(string message)
        {
            var ids = String.Join("|",_requestsPool.Where(m => !string.IsNullOrEmpty(m.Id) && m.Id != "*").Select(m => m.Id));
            var re = new Regex(string.Format(ResponsePattern, ids));
            var match = re.Match(message);
            var id = match.Groups["id"].ToString();
            var status = match.Groups["status"].ToString();
            return string.IsNullOrEmpty(id) 
                ? null
                : new RawObject { Id = id, Status = status };
        }
    }

    
}
