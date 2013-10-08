using System;
using System.Text;
using MicroMail.Infrastructure;
using MicroMail.Services.Pop3.Responses;

namespace MicroMail.Services.Pop3.Commands
{
    class Pop3LoginCommand : ServiceCommandBase<Pop3LoginResponse>
    {
        private const string Command = "APOP {0} {1}";

        public Pop3LoginCommand(string username, string passHash, Action<Pop3LoginResponse> callback) : base(callback)
        {
            var s = string.Format(Command, username, passHash);
            this.Debug(s);
            BinMessage = Encoding.UTF8.GetBytes(s);
        }

        protected override Pop3LoginResponse GenerateResponse(RawObject raw)
        {
            return new Pop3LoginResponse();
        }
    }
}
