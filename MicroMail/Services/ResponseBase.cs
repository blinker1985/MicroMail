namespace MicroMail.Services
{
    abstract class ResponseBase
    {
        public string Status { get; set; }
        public string Body { get; set; }
        public string StatusMessage { get; set; }
        public bool IsSuccessful { get; protected set; }

        public abstract void ParseResponseDetails(string message);
    }

}
