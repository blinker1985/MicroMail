using System;
using System.IO;
using System.Net.Security;
using System.Text;

namespace MicroMail.Services
{
    abstract class ServiceCommandBase<T> : IServiceCommand where T : ResponseBase, new()
    {
        private readonly Action<T> _callback;
        private readonly T _response = new T();

        protected ServiceCommandBase(Action<T> callback)
        {
            _callback = callback;
        }

        public virtual Encoding Encoding {
            get { return Encoding.UTF8; }
        }

        public abstract string Message { get; }

        private void NotifyCallback()
        {
            if (_callback != null)
            {
                _callback(_response);
            }
        }

        public void Execute(SslStream ssl)
        {
            Write(ssl);
            Read(ssl);
            NotifyCallback();
        }

        protected abstract bool IsLastLine(string line);

        protected virtual void Write(SslStream ssl)
        {
            if (string.IsNullOrEmpty(Message)) return;

            var writer = new StreamWriter(ssl);

            writer.WriteLine(Message);
            writer.Flush();
        }

        protected virtual void Read(SslStream ssl)
        {
            var reader = new StreamReader(ssl, Encoding);
            var sb = new StringBuilder();
            string line;
            do
            {
                line = reader.ReadLine();
                sb.AppendLine(line);
            } while (!IsLastLine(line));

            _response.ParseResponseDetails(sb.ToString());
        }

    }
}
