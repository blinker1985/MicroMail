using System;
using System.Security;
using MicroMail.Infrastructure.Helpers;
using MicroMail.Services.Pop3;

namespace MicroMail.Models
{
    [Serializable]
    public class Account
    {
        private readonly string _id;

        public Account()
        {
            _id = Guid.NewGuid().ToString();
        }

        public string Id
        {
            get { return _id; }
        }

        public Type ServiceType {
            get { return typeof(Pop3Service); }
        }

        public string Name { get; set; }
        public string Login { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }

        public string EncryptedPassword;

        public SecureString SecuredPassword
        {
            get { return AccountHelper.ToSecurePassword(AccountHelper.DecryptPassword(EncryptedPassword)); }
        }
    }
}
