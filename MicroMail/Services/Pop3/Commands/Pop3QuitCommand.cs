using System;
using MicroMail.Services.Pop3.Responses;

namespace MicroMail.Services.Pop3.Commands
{
    class Pop3QuitCommand : Pop3SingleLineCommand<Pop3Response>
    {
        public Pop3QuitCommand(Action<Pop3Response> callback)
            : base(callback)
        {

        }

        public override string Message
        {
            get { return "QUIT"; }
        }
    }
}
