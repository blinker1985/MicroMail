using System;

namespace MicroMail.Services.Pop3.Responses
{
    class Pop3InitResponse : ResponseBase
    {
        public string HashSalt { get; private set; }

        public override void ParseResponseDetails(string message)
        {
            var lastSpaceIndex = message.LastIndexOf(" ", StringComparison.InvariantCulture);
            if (lastSpaceIndex > 0)
            {
                HashSalt = message.Substring(lastSpaceIndex).Trim();
            }
        }
    }
}
