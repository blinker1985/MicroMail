using System.Text.RegularExpressions;

namespace MicroMail.Services.Pop3.Responses
{
    class Pop3StatResponse : ResponseBase
    {
        public int Count { get; private set; }

        public override void ParseResponseDetails(string message)
        {
            var re = new Regex("(?<status>.*?)\\s(?<count>[0-9]*?)\\s(?<size>[0-9]*)");
            var match = re.Match(message);

            int c;
            int.TryParse(match.Groups["count"].Value, out c);
            Count = c;
        }

    }
}
