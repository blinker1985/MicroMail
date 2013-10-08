using System;
using System.Linq;
using System.Text.RegularExpressions;
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
        private const string ResponsePattern = "^[^\\](?<id>)\\s(?<status>OK|NO|BAD)";

        public ImapService(EventBus eventBus) : base(eventBus)
        {
        }

        public override void Login()
        {
            SendCommand(new ImapInitCommand(), ServiceStatusEnum.Logging);
            SendCommand(new ImapLoginCommand(Account.Login, AccountHelper.ToInsecurePassword(Account.SecuredPassword), null), ServiceStatusEnum.Logging);
            SendCommand(new ImapSelectRootFolderCommand(null), ServiceStatusEnum.SyncFolder);
        }

        public override void CheckMail()
        {
            SendCommand(new ImapSearchUnseenCommand(CheckMailResponder), ServiceStatusEnum.CheckingMail);
        }

        public override void FetchMailBody(EmailModel email)
        {
            SendCommand(new ImapFetchMailBodyCommand(email, null), ServiceStatusEnum.CheckingMail);
        }

        protected override RawObject GetRawObject(string message)
        {
            var re = new Regex(ResponsePattern);
            var match = re.Match(message);
            var id = match.Groups["id"].ToString();
            var status = match.Groups["status"].ToString();
            return string.IsNullOrEmpty(id)
                ? null
                : new RawObject { Id = id, Status = status };
        }

        private IServiceCommand CreateFetchMailHeadersCommand(string id, Action<EmailModel> callback)
        {
            return new ImapFetchMailHeadersCommand(id, m => { if (callback != null && m != null) callback(m.Email); });
        }

        private void CheckMailResponder(ImapSearchUnseenResponse response)
        {
            UnreadMessagesIds.AddRange(response.UnseenIds);
            FetchMailHeaders(response.UnseenIds);
        }

        private void FetchMailHeaders(string[] ids)
        {
            var count = ids.Count();
            var fetchedCount = 0;
            var sortedIds = ids.Where(m => EmailGroup.EmailList.All(e => e.Id != m)).OrderByDescending(m => m);

            foreach (var id in sortedIds)
            {
                SendCommand(CreateFetchMailHeadersCommand(id, m =>
                {
                    fetchedCount++;

                    if (m == null || EmailGroup.EmailList.Any(e => e.Id == m.Id)) return;

                    m.ServiceId = ServiceId;
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
