using MicroMail.Models;

namespace MicroMail.Infrastructure.Messaging.Events
{
    class FetchMailBodyEvent
    {
        public FetchMailBodyEvent(EmailModel email)
        {
            Email = email;
        }

        public EmailModel Email { get; private set; }
    }
}
