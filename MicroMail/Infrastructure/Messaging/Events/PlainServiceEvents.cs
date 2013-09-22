namespace MicroMail.Infrastructure.Messaging.Events
{
    class PlainServiceEvents
    {
        public const string Connected = "Connected";
        public const string Disconnected = "Disconnected";
        public const string LoginSuccess = "LoginSuccess";
        public const string LoginFailed = "LoginFailed";
        public const string NewMailFetched = "NewMailFetched";
        public const string ServiceFailed = "ServiceFailed";
    }
}
