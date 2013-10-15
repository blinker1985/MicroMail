using System;
using MicroMail.Services.Imap.Responses;

namespace MicroMail.Services.Imap.Commands
{
    class ImapFetchMailHeadersCommand : ImapCommandBase<ImapFetchMailHeaderResponse>
    {
        private const string Command = "FETCH {0} (BODY.PEEK[HEADER.FIELDS (DATE FROM SUBJECT CONTENT-TYPE Content-Transfer-Encoding)])";

        private readonly string _id;

        public ImapFetchMailHeadersCommand(string id, Action<ImapFetchMailHeaderResponse> callback) : base(callback)
        {
            _id = id;
        }

        public override string Message {
            get { return string.Format(Command, _id); }
        }
    }
}
