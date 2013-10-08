using System;
using System.Text;

namespace MicroMail.Services.Pop3.Commands
{
    class Pop3UserCommand : ServiceCommandBase<ResponseBase>
    {
        public Pop3UserCommand(string username, Action<ResponseBase> callback) : base(callback)
        {
            BinMessage = Encoding.UTF8.GetBytes("USER " + username);
        }

        protected override ResponseBase GenerateResponse(RawObject raw)
        {
            return new ResponseBase();
        }
    }
}
