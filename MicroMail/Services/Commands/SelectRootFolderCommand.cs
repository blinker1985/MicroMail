using System;
using MicroMail.Services.Responses;

namespace MicroMail.Services.Commands
{
    class SelectRootFolderCommand : ImapCommand<ResponseBase>
    {
        private const string SelectMessage = "SELECT INBOX";

        public SelectRootFolderCommand(Action<ResponseBase> callback)
            : base(SelectMessage, callback)
        {
        }

        protected override ResponseBase GenerateResponse(RawObject raw)
        {
            return new ResponseBase();
        }
    }
}
