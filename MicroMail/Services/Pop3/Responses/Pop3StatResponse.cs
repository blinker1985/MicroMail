using System.Text.RegularExpressions;

namespace MicroMail.Services.Pop3.Responses
{
    class Pop3StatResponse : Pop3SingleLineResponse
    {
        public int Count { get; private set; }

        protected override void Complete()
        {
            var re = new Regex("(?<status>.*?)\\s(?<count>[0-9]*?)\\s(?<size>[0-9]*)");
            var match = re.Match(ResponseDetails.Body);

            int c;
            int.TryParse(match.Groups["count"].Value, out c);
            Count = c;
        }
    }
}
