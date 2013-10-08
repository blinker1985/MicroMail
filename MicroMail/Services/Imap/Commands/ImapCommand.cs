using System;
using System.Text;
using MicroMail.Infrastructure.Helpers;
using MicroMail.Infrastructure;

namespace MicroMail.Services.Imap.Commands
{
    abstract class ImapCommand<T> : ServiceCommandBase<T> where T : ResponseBase
    {
        //private const string CommandEnding = "\r\n";

        protected ImapCommand(string message, Action<T> callback) : base(callback)
        {
            if (!string.IsNullOrEmpty(message))
            {
                Id = IdGenerator.GenerateId();
                var command = Id + " " + message;
                BinMessage = Encoding.ASCII.GetBytes(command);
            }

            this.Debug(message);
        }
    
    }
}
