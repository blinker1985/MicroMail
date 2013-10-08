using System;
using MicroMail.Services.Imap.Responses;

namespace MicroMail.Services.Imap.Commands
{
    //TODO: It's not secure to store the user's credentials in BinMessage. We shoud use SecureString and create BinMessage on demand to avoid storing the pass in memory.
    class ImapLoginCommand : ImapCommand<ImapLoginResponse>
    {
        private const string LoginMessage = "LOGIN {0} {1}";

        public ImapLoginCommand(string username, string pass, Action<ImapLoginResponse> callback)
            : base(string.Format(LoginMessage, username, pass), callback)
        {

        }

        protected override ImapLoginResponse GenerateResponse(RawObject raw)
        {
            var response = new ImapLoginResponse();
            response.ParseRawResponse(raw);
            return response;
        }
    }
}
