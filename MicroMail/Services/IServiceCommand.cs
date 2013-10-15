using System.Net.Security;
using System.Text;

namespace MicroMail.Services
{
    public interface IServiceCommand
    {
        //byte[] BinMessage { get; }
        Encoding Encoding { get; }
        //string Id { get; }
        void Execute(SslStream stream);
        //void NotifyCallback(RawObject raw);
        //void WriteResponseLine(string line);
        //bool Completed { get; }
    }
}
