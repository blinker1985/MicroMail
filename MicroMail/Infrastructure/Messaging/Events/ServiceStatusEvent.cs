using MicroMail.Services;

namespace MicroMail.Infrastructure.Messaging.Events
{
    class ServiceStatusEvent
    {
        public ServiceStatusEvent(ServiceStatusEnum status)
        {
            Status = status;
        }

        public ServiceStatusEnum Status { get; private set; }
    }
}
