using System;
using MicroMail.Services.Imap.Responses;

namespace MicroMail.Services.Imap.Commands
{
    class ImapSelectRootFolderCommand : ImapCommandBase<ImapSelectRootFolderResponse>
    {
        public ImapSelectRootFolderCommand(Action<ResponseBase> callback)
            : base(callback)
        {
        }

        public override string Message {
            get { return "SELECT INBOX"; }
        }
    }
}
