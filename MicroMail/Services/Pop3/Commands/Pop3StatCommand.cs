using System;
using MicroMail.Services.Pop3.Responses;

namespace MicroMail.Services.Pop3.Commands
{
    class Pop3StatCommand : ServiceCommandBase<Pop3StatResponse>
    {
        public Pop3StatCommand(Action<Pop3StatResponse> callback) : base(callback)
        {
            
        }

        public override string Message
        {
            get { return "STAT"; }
        }
    }
}
