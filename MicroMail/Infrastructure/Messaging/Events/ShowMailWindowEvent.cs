using MicroMail.Models;

namespace MicroMail.Infrastructure.Messaging.Events
{
    class ShowMailWindowEvent
    {
        public ShowMailWindowEvent(EmailModel email)
        {
            Email = email;
        }

        public EmailModel Email { get; private set; }
    }
}
