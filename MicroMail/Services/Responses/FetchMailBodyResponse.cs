using System.Text.RegularExpressions;
using MicroMail.Infrastructure.Helpers;
using MicroMail.Models;

namespace MicroMail.Services.Responses
{
    class FetchMailBodyResponse : ResponseBase
    {
        public string Body { get; set; }

        private const string BodyRegex = "body\\[[0-9a-zA-Z\\.]*\\]\\s\\{[0-9]*\\}\\r\\n(?<body>.*)";

        private readonly EmailModel _email;

        public FetchMailBodyResponse(EmailModel email)
        {
            _email = email;
        }

        public override void ParseRawResponse(RawObject raw)
        {
            if (raw == null) return;
            EmailDecodingHelper.DecodeMailBody(GetResponseSection(raw.Message, BodyRegex, "body"), _email);
        }

        private static string GetResponseSection(string message, string regexString, string groupName)
        {
            var re = new Regex(regexString, RegexOptions.IgnoreCase|RegexOptions.Singleline);
            var match = re.Match(message);
            return match.Groups[groupName].Value;
        }
       
    }
}
