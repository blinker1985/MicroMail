using System;
using MicroMail.Services.Pop3.Responses;

namespace MicroMail.Services.Pop3.Commands
{
    class Pop3UserCommand : Pop3SingleLineCommand<Pop3Response>
    {
        private readonly string _username;

        public Pop3UserCommand(string username, Action<ResponseBase> callback) : base(callback)
        {
            _username = username;
        }

        public override string Message
        {
            get { return "USER " + _username; }
        }
    }
}
