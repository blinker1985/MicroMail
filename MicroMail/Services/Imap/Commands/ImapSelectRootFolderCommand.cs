using System;

namespace MicroMail.Services.Imap.Commands
{
    class ImapSelectRootFolderCommand : ImapCommand<ResponseBase>
    {
        private const string SelectMessage = "SELECT INBOX";

        public ImapSelectRootFolderCommand(Action<ResponseBase> callback)
            : base(SelectMessage, callback)
        {
        }

        protected override ResponseBase GenerateResponse(RawObject raw)
        {
            return new ResponseBase();
        }
    }
}
