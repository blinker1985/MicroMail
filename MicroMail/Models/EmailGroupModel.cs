using System;
using MicroMail.Infrastructure;

namespace MicroMail.Models
{
    [Serializable]
    public class EmailGroupModel
    {
        public EmailGroupModel()
        {
            EmailList = new AsyncObservableCollection<EmailModel>();
        }

        public string AccountId { get; set; }
        public string Name { get; set; }
        public AsyncObservableCollection<EmailModel> EmailList { get; private set; }
    }
}
