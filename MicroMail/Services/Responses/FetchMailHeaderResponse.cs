using MicroMail.Models;
using MicroMail.Infrastructure.Extensions;

namespace MicroMail.Services.Responses
{
    class FetchMailHeaderResponse : ResponseBase
    {
        private readonly string _id;

        public FetchMailHeaderResponse(string id)
        {
            _id = id;
        }

        public EmailModel Email { get; set; }

        public override void ParseRawResponse(RawObject raw)
        {
            if (raw == null) return;
            var contentType = raw.Message.GetHeaderValue("content-type");

            Email = new EmailModel
                {
                    Id = _id,
                    Date = raw.Message.GetHeaderValue("date").ParseDateString(),
                    Subject = raw.Message.GetHeaderValue("subject").DecodeEncodedWord().ReplaceAllNewLines(),
                    From = raw.Message.GetHeaderValue("from").DecodeEncodedWord(),
                    Boundary = raw.Message.GetHeaderValue("content-type", "boundary"),
                    ContentTransferEncoding = raw.Message.GetHeaderValue("content-transfer-encoding"),
                    ContentType = contentType,
                    Charset = raw.Message.GetHeaderValue("content-type", "charset"),
                    IsMultipart = contentType.ToLowerInvariant().Contains("multipart/")
                };
        }

    }
}
