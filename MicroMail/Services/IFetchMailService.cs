using System;
using MicroMail.Models;

namespace MicroMail.Services
{
    interface IFetchMailService
    {
        EmailGroupModel EmailGroup { get; }
        void CheckMail();
        void Start(Account account);
        void Stop();
        void FetchMailBody(EmailModel email);
    }
}
