using System;

namespace MicroMail.Services.Pop3.Responses
{
    abstract class Pop3MultilineResponse : ResponseBase
    {
        protected override bool IsLastLine(string line)
        {
            return line.IndexOf("-ERR", StringComparison.InvariantCulture) == 0 || line.Trim() == ".";
        }

        protected override ResponseDetails ParseResponseDetails(string message)
        {
            return new ResponseDetails();
        }
    }
}
