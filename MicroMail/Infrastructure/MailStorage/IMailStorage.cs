using MicroMail.Models;

namespace MicroMail.Infrastructure.MailStorage
{
    public interface IMailStorage
    {
        void Save(EmailGroupModel[] groups);
        void Load();
        void AddLoadedEmails(EmailGroupModel group);
    }
}
