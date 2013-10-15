using System;
using MicroMail.Services.Pop3.Responses;

namespace MicroMail.Services.Pop3.Commands
{
    class Pop3ListCommand : ServiceCommandBase<Pop3ListResponse>
    {
        public Pop3ListCommand(Action<Pop3ListResponse> callback) : base(callback)
        {
        }

        public override string Message
        {
            get { return "LIST"; }
        }
    }
}
