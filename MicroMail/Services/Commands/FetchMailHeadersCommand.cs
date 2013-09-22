using System;
using MicroMail.Services.Responses;

namespace MicroMail.Services.Commands
{
    class FetchMailHeadersCommand : ImapCommand<FetchMailHeaderResponse>
    {
        private const string Command = "FETCH {0} (BODY.PEEK[HEADER.FIELDS (DATE FROM SUBJECT CONTENT-TYPE Content-Transfer-Encoding)])";

        private readonly string _id;

        public FetchMailHeadersCommand(string id, Action<FetchMailHeaderResponse> callback)
            : base(string.Format(Command, id), callback)
        {
            _id = id;
        }

        protected override FetchMailHeaderResponse GenerateResponse(RawObject raw)
        {
            var response = new FetchMailHeaderResponse(_id);
            response.ParseRawResponse(raw);
            return response;
        }
    }
}
