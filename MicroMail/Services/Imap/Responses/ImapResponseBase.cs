using System;
using System.Text.RegularExpressions;

namespace MicroMail.Services.Imap.Responses
{
    abstract class ImapResponseBase : ResponseBase
    {
        private const string ResponsePattern = "(?<body>.*\\r\\n)*(?<id>.*?)(?<status>NO|OK|BAD)(?<statusMessage>.*)";

        public override void ParseResponseDetails(string message)
        {
            var match = new Regex(ResponsePattern, RegexOptions.Singleline).Match(message);

            Body = match.Groups["body"].Value;
            Status = match.Groups["status"].Value;
            StatusMessage = match.Groups["statusMessage"].Value;

            IsSuccessful = Status.Equals("ok", StringComparison.CurrentCultureIgnoreCase);
        }

    }
}
