using System;
using MicroMail.Models;
using MicroMail.Services.Imap.Responses;

namespace MicroMail.Services.Imap.Commands
{
    class ImapFetchMailBodyCommand : ImapCommand<ImapFetchMailBodyResponse>
    {
        private const string Command = "FETCH {0} BODY[TEXT]";
        private readonly EmailModel _email;
        public ImapFetchMailBodyCommand(EmailModel email, Action<ImapFetchMailBodyResponse> callback) : base(string.Format(Command, email.Id), callback)
        {
            _email = email;
        }

        protected override ImapFetchMailBodyResponse GenerateResponse(RawObject raw)
        {
            var response = new ImapFetchMailBodyResponse(_email);
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
