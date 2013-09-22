using System;
using MicroMail.Services.Responses;

namespace MicroMail.Services.Commands
{
    class SearchUnseenCommand : ImapCommand<SearchUnseenResponse>
    {
        private const string SearchUnseen = "SEARCH UNSEEN";

        public SearchUnseenCommand(Action<SearchUnseenResponse> callback)
            : base(SearchUnseen, callback)
        {

        }

        protected override SearchUnseenResponse GenerateResponse(RawObject raw)
        {
            var response = new SearchUnseenResponse();
            response.ParseRawResponse(raw);
            return response;
        }
    }
}
