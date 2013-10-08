using System;
using MicroMail.Services.Pop3.Responses;

namespace MicroMail.Services.Pop3.Commands
{
    class Pop3InitCommand : ServiceCommandBase<Pop3InitResponse>
    {
        public Pop3InitCommand(Action<Pop3InitResponse> callback) : base(callback)
        {
        }

        protected override Pop3InitResponse GenerateResponse(RawObject raw)
        {
            var response = new Pop3InitResponse();
            response.ParseRawResponse(raw);
            return response;
        }
    }
}
