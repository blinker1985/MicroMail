using System;
using System.IO;
using System.Net.Security;
using MicroMail.Infrastructure;
using MicroMail.Infrastructure.Extensions;
using MicroMail.Infrastructure.Helpers;
using MicroMail.Models;
using MicroMail.Services.Pop3.Responses;

namespace MicroMail.Services.Pop3.Commands
{
    class Pop3ApopLoginCommand : ServiceCommandBase<Pop3SingleLineResponse>
    {
        private readonly Account _account ;
        private readonly string _timestamp ;

        public Pop3ApopLoginCommand(Account account, string timestamp, Action<Pop3SingleLineResponse> callback)
            : base(callback)
        {
            _account = account;
            _timestamp = timestamp;
        }

        public override string Message
        {
            get { return "APOP {0} {1}"; }
        }

        override protected void Write(SslStream ssl)
        {
            if (string.IsNullOrEmpty(Message)) return;

            var writer = new StreamWriter(ssl);
            var passHash = (_timestamp + AccountHelper.ToInsecurePassword(_account.SecuredPassword)).ToMd5HexString();
            writer.WriteLine(Message, _account.Login, passHash);
            writer.Flush();
            this.Debug(Message);
        }

    }
}
