using System.Text;
using MicroMail.Services.Responses;

namespace MicroMail.Services.Commands
{
    interface IImapCommand
    {
        string Id { get;}

        byte[] BinMessage { get; }

        void NotifyCallback(RawObject raw);

        Encoding Encoding { get; }
    }
}
