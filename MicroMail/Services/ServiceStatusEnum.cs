namespace MicroMail.Services
{
    public enum ServiceStatusEnum
    {
        Idle, 
        Logging, 
        SyncFolder,
        CheckingMail, 
        RetreivingHeaders, 
        RetreivingBody,
        Disconnected,
        FailedRead
    }
}
