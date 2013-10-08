using System;
using MicroMail.Services.Imap.Responses;

namespace MicroMail.Services.Imap.Commands
{
    class ImapSearchUnseenCommand : ImapCommand<ImapSearchUnseenResponse>
    {
        private const string SearchUnseen = "SEARCH UNSEEN";

        public ImapSearchUnseenCommand(Action<ImapSearchUnseenResponse> callback)
            : base(SearchUnseen, callback)
        {

        }

        protected override ImapSearchUnseenResponse GenerateResponse(RawObject raw)
        {
            var response = new ImapSearchUnseenResponse();
            response.ParseRawResponse(raw);
            return response;
        }
    }
}
