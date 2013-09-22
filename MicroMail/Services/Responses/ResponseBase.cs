using System;

namespace MicroMail.Services.Responses
{
    public enum RequestStatus
    {
        Pending,
        Ok,
        No,
        Bad
    }

    class ResponseBase
    {
        public string Id { get; private set; }
        public RequestStatus Status { get; private set; }

        public virtual void ParseRawResponse(RawObject raw)
        {
            if (raw == null) return;

            Id = raw.Id;
            RequestStatus s;
            Enum.TryParse(raw.Status, true, out s);
            Status = s;
        }
    }
}
