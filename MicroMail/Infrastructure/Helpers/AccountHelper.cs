using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using MicroMail.Models;

namespace MicroMail.Infrastructure.Helpers
{
    class AccountHelper
    {
        public static string EncryptPassword(string password)
        {
            var arr = ProtectedData.Protect(Encoding.Unicode.GetBytes(password), Encoding.Unicode.GetBytes(Properties.Resources.EncryptionSalt), DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(arr);
        }

        public static string Encode(Account account)
        {
            byte[] arr;
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, account);
                arr = ms.ToArray();
            }
            return Convert.ToBase64String(arr);
        }

        public static string DecryptPassword(string encryptedPass)
        {
            var arr = ProtectedData.Unprotect(Convert.FromBase64String(encryptedPass), Encoding.Unicode.GetBytes(Properties.Resources.EncryptionSalt), DataProtectionScope.CurrentUser);
            return Encoding.Unicode.GetString(arr);
        }

        public static Account Decode(string encryptedAccount)
        {
            var arr = Convert.FromBase64String(encryptedAccount);
            Account obj;
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                ms.Write(arr, 0, arr.Length);
                ms.Seek(0, SeekOrigin.Begin);
                obj = (Account)bf.Deserialize(ms);
            }

            return obj;
        }

        public static SecureString ToSecurePassword(string input)
        {
            var result = new SecureString();

            foreach (var c in input)
            {
                result.AppendChar(c);
            }
            return result;
        }

        public static string ToInsecurePassword(SecureString input)
        {
            string result;

            var pointer = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(input);
            try
            {
                result = System.Runtime.InteropServices.Marshal.PtrToStringBSTR(pointer);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ZeroFreeBSTR(pointer);
            }
            return result;
        }

    }
}
