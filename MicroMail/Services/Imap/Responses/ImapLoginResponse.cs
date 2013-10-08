namespace MicroMail.Services.Imap.Responses
{
    class ImapLoginResponse : ResponseBase
    {
        //TODO: add response parsing to check if the credentials were accepted
        public override void ParseRawResponse(RawObject raw)
        {
            base.ParseRawResponse(raw);
        }
    }
}
