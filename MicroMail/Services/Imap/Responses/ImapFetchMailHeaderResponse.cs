using MicroMail.Models;
using MicroMail.Infrastructure.Extensions;

namespace MicroMail.Services.Imap.Responses
{
    class ImapFetchMailHeaderResponse : ImapResponseBase
    {
        public EmailModel Email { get; set; }

        public override void ParseResponseDetails(string message)
        {
            base.ParseResponseDetails(message);

            if (Status != "OK") return;

            var contentType = Body.GetHeaderValue("content-type");

            Email = new EmailModel
            {
                Date = Body.GetHeaderValue("date").ParseDateString(),
                Subject = Body.GetHeaderValue("subject").DecodeEncodedWord().ReplaceAllNewLines(),
                From = Body.GetHeaderValue("from").DecodeEncodedWord(),
                Boundary = Body.GetHeaderValue("content-type", "boundary"),
                ContentTransferEncoding = Body.GetHeaderValue("content-transfer-encoding"),
                ContentType = contentType,
                Charset = Body.GetHeaderValue("content-type", "charset"),
                IsMultipart = contentType.ToLowerInvariant().Contains("multipart/")
            };
        }

    }
}
