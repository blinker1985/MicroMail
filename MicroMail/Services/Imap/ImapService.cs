using System;
using System.Collections.Generic;
using System.Linq;
using MicroMail.Infrastructure.Helpers;
using MicroMail.Infrastructure.Messaging;
using MicroMail.Infrastructure.Messaging.Events;
using MicroMail.Models;
using MicroMail.Services.Imap.Commands;
using MicroMail.Services.Imap.Responses;

namespace MicroMail.Services.Imap
{
    public class ImapService : FetchMailServiceBase
    {

        public ImapService(EventBus eventBus) : base(eventBus)
        {
        }

        public override void Login()
        {
            SendCommand(new ImapInitCommand());
            SendCommand(new ImapLoginCommand(Account, null));
            SendCommand(new ImapSelectRootFolderCommand(null));
        }

        public override void CheckMail()
        {
            CurrentStatus = ServiceStatusEnum.CheckingMail;
            SendCommand(new ImapSearchUnseenCommand(CheckMailResponder));
        }

        public override void FetchMailBody(EmailModel email)
        {
            SendCommand(new ImapFetchMailBodyCommand(email, r => EmailDecodingHelper.DecodeMailBody(r.MailBody, email)));
        }

        private IServiceCommand CreateFetchMailHeadersCommand(string id, Action<EmailModel> callback)
        {
            return new ImapFetchMailHeadersCommand(id, m =>
                {
                    if (callback == null || m == null) return;

                    m.Email.Id = id;
                    callback(m.Email);
                });
        }

        private void CheckMailResponder(ImapSearchUnseenResponse response)
        {
            FetchMailHeaders(response.UnseenIds);
        }

        private void FetchMailHeaders(IEnumerable<string> ids)
        {
            var sortedIds = ids != null 
                ? ids.Where(m => EmailGroup.EmailList.All(e => e.Id != m)).OrderByDescending(m => m).ToArray()
                : new string[0];
            var count = sortedIds.Count();
            var fetchedCount = 0;

            if (count == 0)
            {
                CurrentStatus = ServiceStatusEnum.Idle;
                return;
            }

            foreach (var id in sortedIds)
            {
                SendCommand(CreateFetchMailHeadersCommand(id, m =>
                {
                    fetchedCount++;

                    if (m == null || EmailGroup.EmailList.Any(e => e.Id == m.Id)) return;

                    m.AccountId = AccountId;
                    EmailGroup.EmailList.Add(m);

                    if (fetchedCount != count) return;

                    CurrentStatus = ServiceStatusEnum.Idle;
                    TriggerEvent(PlainServiceEvents.NewMailFetched);
                }));
            }
        }

    }
}
