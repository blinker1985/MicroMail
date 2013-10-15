using System.Text.RegularExpressions;
using MicroMail.Infrastructure.Extensions;
using MicroMail.Infrastructure.Helpers;
using MicroMail.Models;

namespace MicroMail.Services.Pop3.Responses
{
    class Pop3RetrResponse : Pop3MultilineResponse
    {
        public EmailModel Email { get; set; }

        protected override void Complete()
        {
            var message = ResponseDetails.Body;
            var match = new Regex("(?<headers>.*?)\\r\\n\\r\\n(?<body>.*)\\.", RegexOptions.Singleline).Match(message);

            var headerSection = match.Groups["headers"].Value;
            var bodySection = match.Groups["body"].Value;
            
            var contentType = message.GetHeaderValue("content-type");

            Email = new EmailModel
            {
                Date = headerSection.GetHeaderValue("date").ParseDateString(),
                Subject = headerSection.GetHeaderValue("subject").DecodeEncodedWord().ReplaceAllNewLines(),
                From = headerSection.GetHeaderValue("from").DecodeEncodedWord(),
                Boundary = headerSection.GetHeaderValue("content-type", "boundary"),
                ContentTransferEncoding = headerSection.GetHeaderValue("content-transfer-encoding"),
                ContentType = contentType,
                Charset = headerSection.GetHeaderValue("content-type", "charset"),
                IsMultipart = contentType.ToLowerInvariant().Contains("multipart/")
            };

            EmailDecodingHelper.DecodeMailBody(bodySection, Email);
        }
    }
}
