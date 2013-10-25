using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using MicroMail.Models;
using System.Linq;

namespace MicroMail.Infrastructure.MailStorage
{
    class LocalMailStorage : IMailStorage
    {
        private Dictionary<string, SerializableEmailModel[]> _loadedData;
        private string _applicationDirectory;
        private readonly object _locker = new object();

        public void Load()
        {
            var homePath = Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX
                     ? Environment.GetEnvironmentVariable("HOME")
                     : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
            _applicationDirectory = Path.Combine(homePath ?? "", "MicroMail");
            if (!Directory.Exists(_applicationDirectory))
            {
                Directory.CreateDirectory(_applicationDirectory);
            }

            var path = Path.Combine(_applicationDirectory, "groups.mmd");
            var bf = new BinaryFormatter();
            SerializableEmailModel[] loadedArray;

            using (var fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            using (var zs = new GZipStream(fs, CompressionMode.Decompress))
            {
                loadedArray = zs.BaseStream.Length > 0
                    ? bf.Deserialize(zs) as SerializableEmailModel[]
                    : new SerializableEmailModel[0];
            }

            _loadedData = loadedArray != null
                ? loadedArray.GroupBy(m => m.AccountId).ToDictionary(m => m.Key, m => m.ToArray())
                : new Dictionary<string, SerializableEmailModel[]>();
        }

        public void Save(EmailGroupModel[] groups)
        {
            var savingData = groups.SelectMany(m => m.EmailList).Select(m => new SerializableEmailModel(m)).ToArray();

            if (!savingData.Any()) return;

            var thread = new Thread(() => SaveAsync(savingData));
            thread.Start();
        }

        private void SaveAsync(IEnumerable data)
        {
            lock (_locker)
            {
                var path = Path.Combine(_applicationDirectory, "groups.mmd");
                var bf = new BinaryFormatter();

                using (var ms = new MemoryStream())
                using (var fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                using (var zs = new GZipStream(fs, CompressionLevel.Optimal))
                {
                    bf.Serialize(ms, data);
                    var b = ms.GetBuffer();
                    zs.Write(b, 0, b.Length);
                }
            }
        }

        public void AddLoadedEmails(EmailGroupModel @group)
        {
            if (!_loadedData.ContainsKey(group.AccountId)) return;

            foreach (var loadedEmail in _loadedData[group.AccountId])
            {
                group.EmailList.Add(loadedEmail.ToEmailModel());
            }

            _loadedData.Remove(group.AccountId);
        }

        #region Private Model

        [Serializable]
        private class SerializableEmailModel
        {
            public string AccountId;
            public string Id;
            public string Subject;
            public string Boundary;
            public string ContentTransferEncoding;
            public string ContentType;
            public string Charset;
            public bool IsMultipart;
            public DateTime Date;
            public string From;
            public string MessageId;
            public string InReplyTo;
            public string Body;
            public bool IsRead;

            public SerializableEmailModel(EmailModel emailModel)
            {
                FromEmailModel(emailModel);
            }

            public EmailModel ToEmailModel()
            {
                return new EmailModel
                    {
                        Id = Id,
                        AccountId = AccountId,
                        Subject = Subject,
                        Boundary = Boundary,
                        ContentTransferEncoding = ContentTransferEncoding,
                        ContentType = ContentType,
                        Charset = Charset,
                        IsMultipart = IsMultipart,
                        Date = Date,
                        From = From,
                        MessageId = MessageId,
                        InReplyTo = InReplyTo,
                        Body = Body,
                        IsRead = IsRead
                    };
            }

            public void FromEmailModel(EmailModel model)
            {
                Id = model.Id;
                AccountId = model.AccountId;
                Subject = model.Subject;
                Boundary = model.Boundary;
                ContentTransferEncoding = model.ContentTransferEncoding;
                ContentType = model.ContentType;
                Charset = model.Charset;
                IsMultipart = model.IsMultipart;
                Date = model.Date;
                From = model.From;
                MessageId = model.MessageId;
                InReplyTo = model.InReplyTo;
                Body = model.Body;
                IsRead = model.IsRead;
            }
        }

        #endregion
    }
}
