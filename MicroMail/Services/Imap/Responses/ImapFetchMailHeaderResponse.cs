using MicroMail.Models;
using MicroMail.Infrastructure.Extensions;

namespace MicroMail.Services.Imap.Responses
{
    class ImapFetchMailHeaderResponse : ImapResponseBase
    {
        public EmailModel Email { get; set; }

        protected override void Complete()
        {
            if (ResponseDetails.Status != "OK") return;

            var message = ResponseDetails.Body;
            var contentType = message.GetHeaderValue("content-type");

            Email = new EmailModel
            {
                Date = message.GetHeaderValue("date").ParseDateString(),
                Subject = message.GetHeaderValue("subject").DecodeEncodedWord().ReplaceAllNewLines(),
                From = message.GetHeaderValue("from").DecodeEncodedWord(),
                Boundary = message.GetHeaderValue("content-type", "boundary"),
                ContentTransferEncoding = message.GetHeaderValue("content-transfer-encoding"),
                ContentType = contentType,
                Charset = message.GetHeaderValue("content-type", "charset"),
                IsMultipart = contentType.ToLowerInvariant().Contains("multipart/")
            };
        }
    }
}
