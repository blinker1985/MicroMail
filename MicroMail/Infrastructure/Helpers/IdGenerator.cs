namespace MicroMail.Infrastructure.Helpers
{
    class IdGenerator
    {
        private const string PrefixTemplate = "imap000";

        private static int _prefixIndex;

        public static string GenerateId()
        {
            return PrefixTemplate + ++_prefixIndex;
        }
    }
}
