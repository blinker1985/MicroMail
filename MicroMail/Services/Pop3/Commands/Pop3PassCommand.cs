using System;
using System.IO;
using MicroMail.Infrastructure;
using MicroMail.Infrastructure.Helpers;
using MicroMail.Models;
using MicroMail.Services.Pop3.Responses;
using System.Net.Security;

namespace MicroMail.Services.Pop3.Commands
{
    class Pop3PassCommand : Pop3SingleLineCommand<Pop3Response>
    {
        private readonly Account _account;

        public Pop3PassCommand(Account account, Action<ResponseBase> callback)
            : base(callback)
        {
            _account = account;
        }

        public override string Message {
            get { return "PASS {0}"; }
        }

        protected override void Write(SslStream ssl)
        {
            if (string.IsNullOrEmpty(Message)) return;

            var writer = new StreamWriter(ssl);

            writer.WriteLine(Message, AccountHelper.ToInsecurePassword(_account.SecuredPassword));
            writer.Flush();
            this.Debug(Message);
        }
    }
}
