using System;
using System.Globalization;
using MicroMail.Infrastructure.Messaging;
using MicroMail.Infrastructure.Messaging.Events;
using MicroMail.Models;
using MicroMail.Services.Pop3.Commands;
using MicroMail.Services.Pop3.Responses;
using System.Linq;

namespace MicroMail.Services.Pop3
{
    class Pop3Service : FetchMailServiceBase
    {
        public Pop3Service(EventBus eventBus) : base(eventBus)
        {
        }

        public override void Login()
        {
            SendCommand(new Pop3InitCommand(null), ServiceStatusEnum.Logging);
            //SendCommand(new Pop3ApopLoginCommand(Account, "", null), ServiceStatusEnum.Logging);
            SendCommand(new Pop3UserCommand(Account.Login, null), ServiceStatusEnum.Logging);
            SendCommand(new Pop3PassCommand(Account, null), ServiceStatusEnum.Logging);
        }

        public override void CheckMail()
        {
            SendCommand(new Pop3StatCommand(StatResponder), ServiceStatusEnum.CheckingMail);
        }

        public override void FetchMailBody(EmailModel email)
        {
            if (Account.DeleteReadEmails)
            {
                SendCommand(new Pop3DeleteCommand(email.Id, null), ServiceStatusEnum.RetreivingBody);
            }
        }

        public override void Stop()
        {
            SendCommand(new Pop3QuitCommand(m => base.Stop()), ServiceStatusEnum.Idle);
        }

        private void StatResponder(Pop3StatResponse response)
        {
            var count = response.Count;
            if (count <= 0) return;

            var fetchedCount = 0;

            for (var i = 1; i <= count; i++ )
            {
                var id = i.ToString(CultureInfo.InvariantCulture);

                if (EmailGroup.EmailList.Any(m => m.Id == id)) continue;

                SendCommand(CreateRetrCommand(id, m =>
                    {
                        fetchedCount++;

                        EmailGroup.EmailList.Add(m);
                        if (fetchedCount == count)
                        {
                            TriggetEvent(PlainServiceEvents.NewMailFetched);
                        }
                    }), ServiceStatusEnum.CheckingMail);
            }
        }

        private IServiceCommand CreateRetrCommand(string id, Action<EmailModel> callback)
        {
            return new Pop3RetrCommand(id, m =>
                {
                    m.Email.Id = id;
                    m.Email.AccountId = AccountId;

                    if (callback != null)
                    {
                        callback(m.Email);
                    }
                });
        }

    }
}
