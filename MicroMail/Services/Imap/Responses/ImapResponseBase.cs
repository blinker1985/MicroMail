using System.Text.RegularExpressions;

namespace MicroMail.Services.Imap.Responses
{
    abstract class ImapResponseBase : ResponseBase
    {
        private const string LastLinePattern = "^[^\\\\].*?\\s(OK|NO|BAD)";
        private const string ResponsePattern = "(?<body>.*\\r\\n)*(?<id>.*?)(?<status>NO|OK|BAD)(?<statusMessage>.*)";

        protected override bool IsLastLine(string line)
        {
            return new Regex(LastLinePattern).IsMatch(line);
        }

        protected override ResponseDetails ParseResponseDetails(string message)
        {
            var match = new Regex(ResponsePattern, RegexOptions.Singleline).Match(message);

            return new ResponseDetails
                {
                    Body = match.Groups["body"].Value,
                    Status = match.Groups["status"].Value,
                    StatusMessage = match.Groups["statusMessage"].Value
                };
        }

    }
}
