using System;
using System.Linq;

namespace MicroMail.Services.Imap.Responses
{
    class ImapSearchUnseenResponse : ImapResponseBase
    {
        public string[] UnseenIds { get; set; }

        protected override void Complete()
        {
            if (ResponseDetails.Status != "OK") return;

            var message = ResponseDetails.Body;

            const string startStr = "SEARCH";
            var searchIndex = message.IndexOf(startStr, 0, StringComparison.InvariantCulture);
            UnseenIds = searchIndex <= 0
                ? new string[0]
                : message.Substring(searchIndex + startStr.Length)
                         .Trim()
                         .Split(' ')
                         .Select(m => m.Trim())
                         .Where(m => !string.IsNullOrEmpty(m))
                         .ToArray();
        }
    }
}
