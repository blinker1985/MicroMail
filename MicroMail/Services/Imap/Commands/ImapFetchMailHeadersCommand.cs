using System;
using MicroMail.Services.Imap.Responses;

namespace MicroMail.Services.Imap.Commands
{
    class ImapFetchMailHeadersCommand : ImapCommand<ImapFetchMailHeaderResponse>
    {
        private const string Command = "FETCH {0} (BODY.PEEK[HEADER.FIELDS (DATE FROM SUBJECT CONTENT-TYPE Content-Transfer-Encoding)])";

        private readonly string _id;

        public ImapFetchMailHeadersCommand(string id, Action<ImapFetchMailHeaderResponse> callback)
            : base(string.Format(Command, id), callback)
        {
            _id = id;
        }

        protected override ImapFetchMailHeaderResponse GenerateResponse(RawObject raw)
        {
            var response = new ImapFetchMailHeaderResponse(_id);
            response.ParseRawResponse(raw);
            return response;
        }
    }
}
