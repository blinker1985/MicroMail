namespace MicroMail.Services.Pop3.Responses
{
    class Pop3SingleLineResponse : ResponseBase
    {
        protected override bool IsLastLine(string line)
        {
            return true;
        }

        protected override void Complete()
        {
            
        }

        protected override ResponseDetails ParseResponseDetails(string message)
        {
            return new ResponseDetails();
        }
    }
}
