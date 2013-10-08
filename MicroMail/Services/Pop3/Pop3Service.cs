using System;
using System.Security.Cryptography;
using System.Text;
using MicroMail.Infrastructure.Extensions;
using MicroMail.Infrastructure.Helpers;
using MicroMail.Infrastructure.Messaging;
using MicroMail.Models;
using MicroMail.Services.Pop3.Commands;
using MicroMail.Services.Pop3.Responses;

namespace MicroMail.Services.Pop3
{
    class Pop3Service : FetchMailServiceBase
    {
        public Pop3Service(EventBus eventBus) : base(eventBus)
        {
        }

        public override void Login()
        {
            SendCommand(new Pop3InitCommand(InitResponder), ServiceStatusEnum.Logging);
        }

        public override void CheckMail()
        {
        }

        public override void FetchMailBody(EmailModel email)
        {
        }

        protected override RawObject GetRawObject(string message)
        {
            return new RawObject{Status = "Ok", Message = message};
        }

        private void InitResponder(Pop3InitResponse response)
        {
            var hash = (response.HashSalt + AccountHelper.ToInsecurePassword(Account.SecuredPassword)).ToMd5HexString();
            SendCommand(new Pop3LoginCommand("blinker1985", hash, null), ServiceStatusEnum.Logging);
        }
    }
}
