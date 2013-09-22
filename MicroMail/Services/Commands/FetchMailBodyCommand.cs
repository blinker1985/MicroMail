using System;
using MicroMail.Models;
using MicroMail.Services.Responses;

namespace MicroMail.Services.Commands
{
    class FetchMailBodyCommand : ImapCommand<FetchMailBodyResponse>
    {
        private const string Command = "FETCH {0} BODY[TEXT]";
        private readonly EmailModel _email;
        public FetchMailBodyCommand(EmailModel email, Action<FetchMailBodyResponse> callback) : base(string.Format(Command, email.Id), callback)
        {
            _email = email;
        }

        protected override FetchMailBodyResponse GenerateResponse(RawObject raw)
        {
            var response = new FetchMailBodyResponse(_email);
            response.ParseRawResponse(raw);
            return response;
        }

        public override System.Text.Encoding Encoding
        {
            get
            {
                return string.IsNullOrEmpty(_email.Charset)
                    ? base.Encoding
                    : System.Text.Encoding.GetEncoding(_email.Charset);
            }
        }
    }
}
