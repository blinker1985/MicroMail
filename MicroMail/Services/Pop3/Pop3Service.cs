using System;
using System.Collections.Generic;
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
            SendCommand(new Pop3InitCommand(null));
            //SendCommand(new Pop3ApopLoginCommand(Account, "", null), ServiceStatusEnum.Logging);
            SendCommand(new Pop3UserCommand(Account.Login, null));
            SendCommand(new Pop3PassCommand(Account, null));
        }

        public override void CheckMail()
        {
            CurrentStatus = ServiceStatusEnum.CheckingMail;
            SendCommand(new Pop3StatCommand(StatResponder));
        }

        public override void FetchMailBody(EmailModel email)
        {
            if (Account.DeleteReadEmails)
            {
                SendCommand(new Pop3DeleteCommand(email.Id, null));
            }
        }

        public override void Stop()
        {
            SendCommand(new Pop3QuitCommand(m => base.Stop()));
        }

        private void StatResponder(Pop3StatResponse response)
        {
            var fetchedCount = 0;
            var newIds = new List<string>();

            for (var i = 1; i <= response.Count; i++)
            {
                var id = i.ToString(CultureInfo.InvariantCulture);

                if (EmailGroup.EmailList.All(m => m.Id != id))
                {
                    newIds.Add(id);
                }
            }

            var count = newIds.Count;
            if (count == 0)
            {
                CurrentStatus = ServiceStatusEnum.Idle;
                return;
            }

            foreach (var id in newIds)
            {
                SendCommand(CreateRetrCommand(id, m =>
                {
                    fetchedCount++;

                    EmailGroup.EmailList.Add(m);
                    if (fetchedCount != count) return;

                    CurrentStatus = ServiceStatusEnum.Idle;
                    TriggerEvent(PlainServiceEvents.NewMailFetched);
                }));
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
