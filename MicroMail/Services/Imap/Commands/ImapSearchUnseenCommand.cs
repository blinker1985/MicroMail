using System;
using MicroMail.Services.Imap.Responses;

namespace MicroMail.Services.Imap.Commands
{
    class ImapSearchUnseenCommand : ImapCommandBase<ImapSearchUnseenResponse>
    {
        public ImapSearchUnseenCommand(Action<ImapSearchUnseenResponse> callback) : base(callback)
        {

        }

        public override string Message {
            get { return "SEARCH UNSEEN"; }
        }
    }
}
