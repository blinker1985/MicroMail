using System;
using MicroMail.Models;
using MicroMail.Services.Imap.Responses;

namespace MicroMail.Services.Imap.Commands
{
    class ImapFetchMailBodyCommand : ImapCommandBase<ImapFetchMailBodyResponse>
    {
        private readonly EmailModel _email;
        public ImapFetchMailBodyCommand(EmailModel email, Action<ImapFetchMailBodyResponse> callback) : base(callback)
        {
            _email = email;
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

        public override string Message
        {
            get { return string.Format("FETCH {0} BODY[TEXT]", _email.Id); }
        }
    }
}
