using MicroMail.Infrastructure;

namespace MicroMail.Models
{
    public class EmailGroupModel
    {
        public EmailGroupModel()
        {
            EmailList = new AsyncObservableCollection<EmailModel>();
        }

        public string Name { get; set; }
        public AsyncObservableCollection<EmailModel> EmailList { get; private set; }
    }
}
