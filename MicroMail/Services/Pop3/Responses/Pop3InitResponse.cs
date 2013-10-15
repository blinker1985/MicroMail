using System;

namespace MicroMail.Services.Pop3.Responses
{
    class Pop3InitResponse : Pop3SingleLineResponse
    {
        public string HashSalt { get; private set; }

        protected override bool IsLastLine(string line)
        {
            return true;
        }

        protected override void Complete()
        {
            var message = ResponseDetails.Body;
            var lastSpaceIndex = message.LastIndexOf(" ", StringComparison.InvariantCulture);
            if (lastSpaceIndex > 0)
            {
                HashSalt = message.Substring(lastSpaceIndex).Trim();
            }
        }
    }
}
