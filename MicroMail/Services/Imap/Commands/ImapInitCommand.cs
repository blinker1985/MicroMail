using MicroMail.Services.Imap.Responses;

namespace MicroMail.Services.Imap.Commands
{
    class ImapInitCommand : ServiceCommandBase<ImapInitResponse>
    {
        public ImapInitCommand() : base(null)
        {
        }

        public override string Message
        {
            get { return null; }
        }
    }
}
