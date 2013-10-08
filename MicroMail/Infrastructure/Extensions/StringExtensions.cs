using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using MicroMail.Infrastructure.Helpers;
using MicroMail.Models;
using MicroMail.Services;
using MicroMail.Services.Pop3.Commands;

namespace MicroMail.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        public static string GetHeaderValue(this string body, string name, string attribute = null)
        {
            var re = string.IsNullOrEmpty(attribute)
                ? string.Format("{0}:(?<result>.*?)(;|\\r\\n[^ ]|$)", name)
                : string.Format("{0}:.*?\\s*{1}=\\\\?\"?(?<result>.*?)\\\\?\"?(;|\\r\\n[^ ]|$)", name, attribute);
            var match = new Regex(re, RegexOptions.IgnoreCase|RegexOptions.Singleline).Match(body);
            return match.Groups["result"].Value.Trim();
        }

        public static string DecodeEncodedWord(this string word)
        {
            var re = new Regex("=\\?(?<encoding>.*?)\\?(?<type>b|q)\\?(?<text>.*?)\\?=", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var matches = re.Matches(word);

            foreach (Match match in matches)
            {
                var type = match.Groups["type"].Value.ToLower();
                var text = match.Groups["text"].Value;
                var encoding = match.Groups["encoding"].Value;
                var decodedStr = type == "b" ? DecodeBase64(text, encoding) : DecodeQuotedPrintable(text, encoding);
                word = word.Replace(match.Value, decodedStr);
            }

            return word;
        }

        public static string DecodeBase64(this string text, string charset)
        {
            var ba = Convert.FromBase64String(text);
            var decodedBa = Encoding.Convert(Encoding.GetEncoding(charset), Encoding.Unicode, ba);
            return Encoding.Unicode.GetString(decodedBa);
        }

        public static string DecodeQuotedPrintable(this string text, string charset)
        {
            text = new Regex("=\r\n").Replace(text, "");
            var replacementRe = new Regex("(=[0-9A-F][0-9A-F])+", RegexOptions.IgnoreCase);
            return replacementRe.Replace(text, match => HexEvaluator(match, charset));
        }

        private static string HexEvaluator(Match match, string charset)
        {
            var hexes = match.Groups[0].Value.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
            var bytes = new byte[hexes.Count()];
            for (var i = 0; i < hexes.Count(); i++)
            {
                var iInt = Convert.ToInt32(hexes[i], 16);
                bytes[i] = Convert.ToByte(iInt);
            }

            return Encoding.GetEncoding(charset).GetString(bytes);
        }

        public static DateTime ParseDateString(this string dateStr)
        {
            var match = new Regex("(?<date>.*?)\\s*(\\((?<standard>[a-z0-9]{2,3})\\))*$", RegexOptions.IgnoreCase).Match(dateStr);
            DateTime date;
            DateTime.TryParse(match.Groups["date"].Value, out date);

            return date;
        }

        public static string ReplaceAllNewLines(this string str, string replacement = "")
        {
            var re = new Regex("(\\r\\n|\\r|\\n)");
            return re.Replace(str, replacement);
        }

        public static string EscapeRegexpSpecialChars(this string str)
        {
            return new Regex("[\\\\\\.\\+\\*\\?\\^\\$\\[\\]\\(\\)\\{\\}\\/\\|\\'\\#]").Replace(str, "\\$&");
        }

        public static string ToMd5HexString(this string str)
        {
            var md5 = MD5.Create();
            var hashBin = md5.ComputeHash(Encoding.UTF8.GetBytes(str));

            return hashBin.Aggregate("", (current, b) => current + b.ToString("x2"));
        }
    }
}
