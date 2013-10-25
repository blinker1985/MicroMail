using System;
using System.Linq;

namespace MicroMail.Services.Imap.Responses
{
    class ImapSearchUnseenResponse : ImapResponseBase
    {
        public string[] UnseenIds { get; set; }

        public override void ParseResponseDetails(string message)
        {
            base.ParseResponseDetails(message);

            if (Status != "OK") return;

            const string startStr = "SEARCH";
            var searchIndex = Body.IndexOf(startStr, 0, StringComparison.InvariantCulture);
            UnseenIds = searchIndex <= 0
                ? new string[0]
                : Body.Substring(searchIndex + startStr.Length)
                         .Trim()
                         .Split(' ')
                         .Select(m => m.Trim())
                         .Where(m => !string.IsNullOrEmpty(m))
                         .ToArray();
        }

    }
}
