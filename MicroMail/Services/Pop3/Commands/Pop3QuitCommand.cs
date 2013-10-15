using System;
using MicroMail.Services.Pop3.Responses;

namespace MicroMail.Services.Pop3.Commands
{
    class Pop3QuitCommand : ServiceCommandBase<Pop3SingleLineResponse>
    {
        public Pop3QuitCommand(Action<Pop3SingleLineResponse> callback) : base(callback)
        {

        }

        public override string Message
        {
            get { return "QUIT"; }
        }
    }
}
