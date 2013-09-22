using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using MicroMail.Models;
using MicroMail.Infrastructure.Extensions;

namespace MicroMail.Infrastructure.Helpers
{
    public static class EmailDecodingHelper
    {
        private const string HtmlContentType = "text/html";
        private const string MailPartDividerPattern = "(?<=--{0})(?<content>.*?)(?=--{0})";
        private const string MailPartDetailsPattern = "(?<body>(?<=\\r\\n\\r\\n).*)";

        public static void DecodeMailBody(string body, EmailModel email)
        {
            var emailBody = email.IsMultipart 
                ? ProcessMailPartsRecursively(body, email.Boundary) 
                : ProcessSinglePartEmail(body, email);

            if (emailBody == null) return;

            switch (emailBody.ContentEncoding.ToLower())
            {
                case "quoted-printable":
                    emailBody.Content = emailBody.Content.DecodeQuotedPrintable(emailBody.Charset);
                    break;

                case "base64":
                    emailBody.Content = emailBody.Content.DecodeBase64(emailBody.Charset);
                    break;

                default:
                    emailBody.Content = emailBody.Content;
                    break;
            }

            emailBody.Content = FixHtml(emailBody.Content, emailBody.Charset);
            email.Charset = emailBody.Charset;
            email.Body = emailBody.Content;
        }

        private static EmailBody ProcessSinglePartEmail(string body, EmailModel email)
        {
            return new EmailBody
                {
                    Content = body,
                    ContentEncoding = email.ContentTransferEncoding,
                    ContentType = email.ContentType,
                    Charset = email.Charset
                };
        }

        private static EmailBody ProcessMailPartsRecursively(string partBody, string boundary, int recursionLevel = 0)
        {
            boundary = boundary.EscapeRegexpSpecialChars();
            var re = new Regex(string.Format(MailPartDividerPattern, boundary), RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var parts = re.Matches(partBody);
            var count = parts.Count;
            EmailBody result = null;

            for (var i = count - 1; i >= 0; i--)
            {
                var part = parts[i];
                var contentType = part.Value.GetHeaderValue("content-type");

                if (IsMultipartContent(contentType))
                {
                    var subBoundary = part.Value.GetHeaderValue("content-type", "boundary");
                    result = ProcessMailPartsRecursively(part.Value, subBoundary, ++recursionLevel);
                }

                if (result != null)
                {
                    break;
                }

                if (!contentType.Equals(HtmlContentType, StringComparison.InvariantCultureIgnoreCase) && (i > 0 || recursionLevel != 0)) continue;

                var match = new Regex(MailPartDetailsPattern, RegexOptions.Singleline).Match(part.Value);
                result = new EmailBody
                {
                    Content = match.Value,
                    ContentType = contentType,
                    ContentEncoding = part.Value.GetHeaderValue("content-transfer-encoding"),
                    Charset = part.Value.GetHeaderValue("content-type", "charset")
                };

                break;
            }
            return result;

        }

        private static bool IsMultipartContent(string body)
        {
            return body.IndexOf("multipart/", 0, StringComparison.InvariantCultureIgnoreCase) >= 0;
        }

        public static IDictionary<string, string> ParseToDictionary(this string text, string regexPattern)
        {
            var re = new Regex(regexPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var names = re.GetGroupNames().Where(m =>
                {
                    int i;
                    return !int.TryParse(m, out i);
                }).ToArray();
            var matches = re.Matches(text);
            var result = new Dictionary<string, string>(); 
            foreach (Match match in matches)
            {
                foreach (var name in names)
                {
                    if (match.Groups[name].Success && !result.ContainsKey(name))
                    {
                        result[name] = match.Groups[name].Value;
                    }
                }
            }
            return result;
        }

        private static string FixHtml(string content, string charset)
        {
            var contentTypeCheck = new Regex("<meta http-equiv=\"content-type\" content=\"text\\/html;\\s*charset=.*?>");
            var charsetCheck = new Regex("<meta charset=\".*?\">");

            if (contentTypeCheck.Match(content).Success || charsetCheck.Match(content).Success)
            {
                return content;
            }

            var meta = string.Format("<meta http-equiv=\"content-type\" content=\"text/html;charset={0}\"/>", charset);
            var bodyIndex = content.IndexOf("<body", 0, StringComparison.InvariantCultureIgnoreCase);
            var htmlIndex = content.IndexOf("<html", 0, StringComparison.InvariantCultureIgnoreCase);

            if (bodyIndex < 0)
            {
                content = string.Format("<body>\r\n{0}\r\n</body>", content);
            }

            var insertIndex = content.IndexOf("</head>", 0, StringComparison.InvariantCultureIgnoreCase);

            if (insertIndex < 0)
            {
                meta = string.Format("<head>\r\n{0}\r\n</head>", meta);
                insertIndex = content.IndexOf("<body", 0, StringComparison.InvariantCultureIgnoreCase);
            }
            content = content.Insert(insertIndex, meta);

            if (htmlIndex < 0)
            {
                content = string.Format("<html>\r\n{0}\r\n</html>", content);
            }

            return content;
        }

    }
}
