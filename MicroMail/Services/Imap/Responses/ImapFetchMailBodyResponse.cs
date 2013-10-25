using System.Text.RegularExpressions;

namespace MicroMail.Services.Imap.Responses
{
    class ImapFetchMailBodyResponse : ImapResponseBase
    {
        public string MailBody { get; set; }

        private const string BodyRegex = "\\(.*?body\\[[0-9a-zA-Z\\.]*\\]\\s\\{[0-9]*\\}\\r\\n(?<body>.*)\\)";

        public override void ParseResponseDetails(string message)
        {
            base.ParseResponseDetails(message);

            if (Status != "OK") return;

            var re = new Regex(BodyRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var match = re.Match(Body);
            MailBody = match.Groups["body"].Value;
        }

    }
}
