using System;
using System.Linq;

namespace MicroMail.Services.Imap.Responses
{
    class ImapSearchUnseenResponse : ResponseBase
    {
        public string[] UnseenIds { get; set; }

        public override void ParseRawResponse(RawObject raw)
        {
            const string startStr = "SEARCH";
            var searchIndex = raw.Message.IndexOf(startStr, 0, StringComparison.InvariantCulture);
            UnseenIds = searchIndex >= 0 
                ? raw.Message
                     .Substring(searchIndex + startStr.Length)
                     .Trim()
                     .Split(' ')
                     .Select(m => m.Trim())
                     .Where(m => !string.IsNullOrEmpty(m))
                     .ToArray()
                : new string[0];

            base.ParseRawResponse(raw);
        }
    }
}
