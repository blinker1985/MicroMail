using System.Text;

namespace MicroMail.Services
{
    public interface IServiceCommand
    {
        byte[] BinMessage { get; }
        Encoding Encoding { get; }
        string Id { get; }
        void NotifyCallback(RawObject raw);
    }
}
