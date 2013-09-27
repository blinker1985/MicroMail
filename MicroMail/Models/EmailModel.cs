using System;

namespace MicroMail.Models
{
    public class EmailModel : BindableModel
    {
        public string Id { get; set; }
        public string ServiceId { get; set; }
        public string Subject { get; set; }
        public string Boundary { get; set; }
        public string ContentTransferEncoding { get; set; }
        public string ContentType { get; set; }
        public string Charset { get; set; }
        public bool IsMultipart { get; set; }
        public DateTime Date { get; set; }
        public string From { get; set; }
        public string MessageId { get; set; }
        public string InReplyTo { get; set; }

        private string _body;
        public string Body
        {
            get { return _body; }
            set { SetProperty(ref _body, value); }
        }

        private bool _isRead;
        public bool IsRead {
            get { return _isRead; }
            set { SetProperty(ref _isRead, value);}
        }

    }
}
