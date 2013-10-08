using System;
using System.Text;

namespace MicroMail.Services
{
    abstract class ServiceCommandBase<T> : IServiceCommand where T : ResponseBase
    {
        private readonly Action<T> _callback;

        protected ServiceCommandBase(Action<T> callback)
        {
            _callback = callback;
        }

        public string Id { get; protected set; }

        public byte[] BinMessage { get; protected set; }

        public virtual Encoding Encoding {
            get { return Encoding.Default; }
        }

        public void NotifyCallback(RawObject raw)
        {
            var response = GenerateResponse(raw);
            if (_callback != null)
            {
                _callback(response);
            }
        }

        protected abstract T GenerateResponse(RawObject raw);
    }
}
