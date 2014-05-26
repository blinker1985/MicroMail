using System;
using MicroMail.Services.Imap.Responses;

namespace MicroMail.Services.Imap.Commands
{
    class ImapInitCommand : ServiceCommandBase<ImapInitResponse>
    {
        public ImapInitCommand(Action<ImapInitResponse> callback) : base(callback)
        {
        }

        public override string Message
        {
            get { return null; }
        }

        protected override bool IsLastLine(string line)
        {
            return true;
        }
    }
}
