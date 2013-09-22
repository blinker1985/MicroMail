using System;
using MicroMail.Services.Responses;

namespace MicroMail.Services.Commands
{
    class LoginCommand : ImapCommand<LoginResponse>
    {
        private const string LoginMessage = "LOGIN {0} {1}";

        public LoginCommand(string username, string pass, Action<LoginResponse> callback)
            : base(string.Format(LoginMessage, username, pass), callback)
        {

        }

        protected override LoginResponse GenerateResponse(RawObject raw)
        {
            var response = new LoginResponse();
            response.ParseRawResponse(raw);
            return response;
        }
    }
}
