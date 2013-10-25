using System.Net.Security;
using System.Text;

namespace MicroMail.Services
{
    public interface IServiceCommand
    {
        Encoding Encoding { get; }
        void Execute(SslStream stream);
    }
}
