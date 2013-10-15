using System;
using MicroMail.Services.Pop3.Responses;

namespace MicroMail.Services.Pop3.Commands
{
    class Pop3RetrCommand : ServiceCommandBase<Pop3RetrResponse>
    {
        private readonly string _id ;
        public Pop3RetrCommand(string id, Action<Pop3RetrResponse> callback) : base(callback)
        {
            _id = id;
        }

        public override string Message
        {
            get { return "RETR " + _id; }
        }
    }
}
