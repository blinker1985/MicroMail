namespace MicroMail.Services.Imap.Commands
{
    class ImapInitCommand : ServiceCommandBase<ResponseBase>
    {
        public ImapInitCommand() : base(null)
        {
        }

        protected override ResponseBase GenerateResponse(RawObject raw)
        {
            return new ResponseBase();
        }
    }
}
