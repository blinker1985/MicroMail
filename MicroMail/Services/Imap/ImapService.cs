using System;
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
            SendCommand(new ImapInitCommand(), ServiceStatusEnum.Logging);
            SendCommand(new ImapLoginCommand(Account, null), ServiceStatusEnum.Logging);
            SendCommand(new ImapSelectRootFolderCommand(null), ServiceStatusEnum.SyncFolder);
        }

        public override void CheckMail()
        {
            SendCommand(new ImapSearchUnseenCommand(CheckMailResponder), ServiceStatusEnum.CheckingMail);
        }

        public override void FetchMailBody(EmailModel email)
        {
            SendCommand(new ImapFetchMailBodyCommand(email, r => EmailDecodingHelper.DecodeMailBody(r.Body, email)), ServiceStatusEnum.CheckingMail);
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

        private void FetchMailHeaders(string[] ids)
        {
            if (ids == null) return;

            var count = ids.Count();
            var fetchedCount = 0;
            var sortedIds = ids.Where(m => EmailGroup.EmailList.All(e => e.Id != m)).OrderByDescending(m => m);

            foreach (var id in sortedIds)
            {
                SendCommand(CreateFetchMailHeadersCommand(id, m =>
                {
                    fetchedCount++;

                    if (m == null || EmailGroup.EmailList.Any(e => e.Id == m.Id)) return;

                    m.AccountId = AccountId;
                    EmailGroup.EmailList.Add(m);

                    if (fetchedCount == count)
                    {
                        TriggetEvent(PlainServiceEvents.NewMailFetched);
                    }
                }), ServiceStatusEnum.RetreivingHeaders);
            }
        }

    }
}
