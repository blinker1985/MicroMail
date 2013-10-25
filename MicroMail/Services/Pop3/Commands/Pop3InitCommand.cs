using System;
using MicroMail.Services.Pop3.Responses;

namespace MicroMail.Services.Pop3.Commands
{
    class Pop3InitCommand : Pop3SingleLineCommand<Pop3InitResponse>
    {
        public Pop3InitCommand(Action<Pop3InitResponse> callback) : base(callback)
        {
        }

        public override string Message
        {
            get { return null; }
        }
    }
}
