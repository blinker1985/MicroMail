using MicroMail.Models;

namespace MicroMail.Services
{
    interface IFetchMailService
    {
        EmailGroupModel EmailGroup { get; }
        void CheckMail();
        void Init(Account account);
        void Start();
        void Stop();
        void FetchMailBody(EmailModel email);
        ServiceStatusEnum CurrentStatus { get; }
    }
}
