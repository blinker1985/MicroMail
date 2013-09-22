using System;
using System.Text;
using MicroMail.Infrastructure.Helpers;
using MicroMail.Services.Responses;

namespace MicroMail.Services.Commands
{
    abstract class ImapCommand<T> : IImapCommand where T : ResponseBase
    {
        private const string CommandEnding = "\r\n";

        protected ImapCommand(string message, Action<T> callback)
        {
            if (!string.IsNullOrEmpty(message))
            {
                Id = IdGenerator.GenerateId();
                var command = Id + " " + message + CommandEnding;
                Console.WriteLine(command);
                BinMessage = Encoding.ASCII.GetBytes(command);
            }
            Callback = callback;
        }

        public string Id { get; private set; }

        public byte[] BinMessage { get; private set; }

        private Action<T> Callback { get; set; }

        public void NotifyCallback(RawObject raw)
        {
            var response = GenerateResponse(raw);
            if (Callback != null)
            {
                Callback(response);
            }
        }

        virtual public Encoding Encoding {
            get { return Encoding.Default; }
        }

        protected abstract T GenerateResponse(RawObject raw);
    
    }
}
