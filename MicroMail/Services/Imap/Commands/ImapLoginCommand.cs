using System;
using System.IO;
using MicroMail.Infrastructure;
using MicroMail.Infrastructure.Helpers;
using MicroMail.Models;
using MicroMail.Services.Imap.Responses;

namespace MicroMail.Services.Imap.Commands
{
    class ImapLoginCommand : ImapCommandBase<ImapLoginResponse>
    {
        private readonly Account _account ;

        public ImapLoginCommand(Account account, Action<ImapLoginResponse> callback) : base(callback)
        {
            _account = account;
        }

        public override string Message
        {
            get { return "{0} LOGIN {1} {2}"; }
        }

        protected override void Write(System.Net.Security.SslStream ssl)
        {
            if (string.IsNullOrEmpty(Message)) return;

            var writer = new StreamWriter(ssl);
            writer.WriteLine(Message, IdGenerator.GenerateId(), _account.Login, AccountHelper.ToInsecurePassword(_account.SecuredPassword));
            writer.Flush();
            this.Debug(string.Format(Message, "", _account.Login, "*******"));
        }
    }
}
