namespace MicroMail.Models
{
    class EmailBody
    {
        public string ContentType { get; set; }
        public string ContentEncoding { get; set; }
        public string Content { get; set; }
        public string Charset { get; set; }
    }
}
