using System.Text;

namespace MicroMail.Services
{
    abstract class ResponseBase
    {
        public bool Completed { get; protected set; }

        protected ResponseDetails ResponseDetails { get; set; }

        private string _message;

        public void WriteLine(string line)
        {
            _message = new StringBuilder(_message).AppendLine(line).ToString();

            if (!IsLastLine(line)) return;

            ResponseDetails = ParseResponseDetails(_message);
            Complete();
            Completed = true;
        }

        protected abstract bool IsLastLine(string line);
        protected abstract void Complete();
        protected abstract ResponseDetails ParseResponseDetails(string message);
    }

    internal class ResponseDetails
    {
        public string Status { get; set; }
        public string Body { get; set; }
        public string StatusMessage { get; set; }
    }
}
