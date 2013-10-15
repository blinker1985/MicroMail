using System.Text.RegularExpressions;

namespace MicroMail.Services.Imap.Responses
{
    class ImapFetchMailBodyResponse : ImapResponseBase
    {
        public string Body { get; set; }

        private const string BodyRegex = "\\(.*?body\\[[0-9a-zA-Z\\.]*\\]\\s\\{[0-9]*\\}\\r\\n(?<body>.*)\\)";

        protected override void Complete()
        {
            if (ResponseDetails.Status == "OK")
            {
                Body = GetResponseSection(ResponseDetails.Body, BodyRegex, "body");
            }
        }

        private static string GetResponseSection(string message, string regexString, string groupName)
        {
            var re = new Regex(regexString, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var match = re.Match(message);
            return match.Groups[groupName].Value;
        }
       
    }
}
