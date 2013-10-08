using System;
using System.Text;

namespace MicroMail.Services.Pop3.Commands
{
    class Pop3PassCommand : ServiceCommandBase<ResponseBase>
    {
        public Pop3PassCommand(string pass, Action<ResponseBase> callback)
            : base(callback)
        {
            BinMessage = Encoding.UTF8.GetBytes("PASS " + pass);
        }

        protected override ResponseBase GenerateResponse(RawObject raw)
        {
            return new ResponseBase();
        }
    }
}
