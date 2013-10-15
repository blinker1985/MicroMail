using System;
using System.Security;
using MicroMail.Infrastructure.Helpers;
using MicroMail.Services.Pop3;

namespace MicroMail.Models
{
    public enum ProtocolTypeEnum
    {
        Imap, Pop3
    }

    [Serializable]
    public class Account
    {
        public Account()
        {
            Id = Guid.NewGuid().ToString();
        }

        //TODO: incapsulate the setter.
        public string Id { get; set; }

        public string Name { get; set; }
        public string Login { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }

        public bool DeleteReadEmails { get; set; }
        public bool SaveEmailsLocally { get; set; }

        public ProtocolTypeEnum ProtocolType { get; set; }

        public string EncryptedPassword;

        public SecureString SecuredPassword
        {
            get { return AccountHelper.ToSecurePassword(AccountHelper.DecryptPassword(EncryptedPassword)); }
        }
    }
}
