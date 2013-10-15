using MicroMail.Models;

namespace MicroMail.Infrastructure.MailStorage
{
    public interface IMailStorage
    {
        void WatchEmailGroup(EmailGroupModel group);
        void UnwatchEmailGroup(EmailGroupModel group);
    }
}
