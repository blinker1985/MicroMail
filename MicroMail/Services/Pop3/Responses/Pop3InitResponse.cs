using System;

namespace MicroMail.Services.Pop3.Responses
{
    class Pop3InitResponse : ResponseBase
    {
        public string HashSalt { get; private set; }

        public override void ParseRawResponse(RawObject raw)
        {
            base.ParseRawResponse(raw);
            var lastSpaceIndex = raw.Message.LastIndexOf(" ", StringComparison.InvariantCulture);
            if(lastSpaceIndex > 0)
            {
                HashSalt = raw.Message.Substring(lastSpaceIndex).Trim();
            }
        }
    }
}
