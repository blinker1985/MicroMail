using System;
using System.Linq;

namespace MicroMail.Services.Responses
{
    class SearchUnseenResponse : ResponseBase
    {
        public string[] UnseenIds { get; set; }

        public override void ParseRawResponse(RawObject raw)
        {
            const string startStr = "SEARCH";
            var startIndex = raw.Message.IndexOf(startStr, 0, StringComparison.InvariantCulture) + startStr.Length;
            UnseenIds = raw.Message
                           .Substring(startIndex)
                           .Trim()
                           .Split(' ')
                           .Select(m => m.Trim())
                           .Where(m => !string.IsNullOrEmpty(m))
                           .ToArray();

            base.ParseRawResponse(raw);
        }
    }
}
