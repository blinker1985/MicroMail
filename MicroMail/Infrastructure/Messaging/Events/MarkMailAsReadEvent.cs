using MicroMail.Models;

namespace MicroMail.Infrastructure.Messaging.Events
{
    public class MarkMailAsReadEvent
    {
        public MarkMailAsReadEvent(EmailModel email)
        {
            Email = email;
        }

        public EmailModel Email { get; set; }
    }
}
