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
        private string lastUid;
        public ImapService(EventBus eventBus) : base(eventBus)
        {
        }

        public override void Login()
        {
            SendCommandAsync(new ImapInitCommand(null));
            SendCommandAsync(new ImapLoginCommand(Account, null));
            
        }

        protected override bool TestLogin()
        {
            var result = false;
            SendCommandSync(new ImapInitCommand(i =>
                {
                    if (i.IsSuccessful)
                    {
                        SendCommandSync(new ImapLoginCommand(Account, l => result = l.IsSuccessful));
                    }
                }));

            return result;
        }

        public override void CheckMail()
        {
            CurrentStatus = ServiceStatusEnum.CheckingMail;
            SendCommandAsync(new ImapSelectRootFolderCommand(i => SendCommandAsync(new ImapSearchUnseenCommand(CheckMailResponder))));
        }

        public override void FetchMailBody(EmailModel email)
        {
            SendCommandAsync(new ImapFetchMailBodyCommand(email, r => EmailDecodingHelper.DecodeMailBody(r.MailBody, email)));
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
            lastUid = response.UnseenIds.LastOrDefault();
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
                SendCommandAsync(CreateFetchMailHeadersCommand(id, m =>
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
