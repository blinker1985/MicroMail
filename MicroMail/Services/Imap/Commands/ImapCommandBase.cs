using System;
using System.IO;
using System.Net.Security;
using System.Text.RegularExpressions;
using MicroMail.Infrastructure.Helpers;

namespace MicroMail.Services.Imap.Commands
{
    abstract class ImapCommandBase<T> : ServiceCommandBase<T> where T : ResponseBase, new()
    {
        private const string LastLinePattern = "{0}.*?\\s(OK|NO|BAD)";
        private string _id;

        protected ImapCommandBase(Action<T> callback) : base(callback)
        {
        }

        protected override bool IsLastLine(string line)
        {
            return new Regex(string.Format(LastLinePattern, _id)).IsMatch(line);
        }

        override protected void Write(SslStream ssl)
        {
            if (string.IsNullOrEmpty(Message)) return;
            _id = IdGenerator.GenerateId();
            var writer = new StreamWriter(ssl);

            writer.WriteLine("{0} {1}", _id, Message);
            writer.Flush();
        }
    }
}
