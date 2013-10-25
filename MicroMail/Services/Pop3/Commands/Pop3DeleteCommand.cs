using System;
using MicroMail.Services.Pop3.Responses;

namespace MicroMail.Services.Pop3.Commands
{
    class Pop3DeleteCommand : Pop3SingleLineCommand<Pop3Response>
    {
        private readonly string _id;
        public Pop3DeleteCommand(string id, Action<Pop3Response> callback)
            : base(callback)
        {
            _id = id;
        }

        public override string Message
        {
            get { return string.Format("DELE {0}", _id); }
        }
    }
}
