using System;
using System.IO;
using System.Net.Security;
using MicroMail.Infrastructure.Helpers;

namespace MicroMail.Services.Imap.Commands
{
    abstract class ImapCommandBase<T> : ServiceCommandBase<T> where T : ResponseBase, new()
    {
        protected ImapCommandBase(Action<T> callback) : base(callback)
        {
        }

        override protected void Write(SslStream ssl)
        {
            if (string.IsNullOrEmpty(Message)) return;

            var writer = new StreamWriter(ssl);

            writer.WriteLine("{0} {1}", IdGenerator.GenerateId(), Message);
            writer.Flush();
        }
    }
}
