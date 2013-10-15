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
            get { return Encoding.Default; }
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

            while (!_response.Completed)
            {
                _response.WriteLine(reader.ReadLine());
            }
        }

    }
}
