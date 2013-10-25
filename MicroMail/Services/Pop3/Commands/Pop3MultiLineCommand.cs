using System;

namespace MicroMail.Services.Pop3.Commands
{
    abstract class Pop3MultiLineCommand<T> : ServiceCommandBase<T> where T: ResponseBase, new()
    {
        protected Pop3MultiLineCommand(Action<T> callback) : base(callback)
        {

        }

        protected override bool IsLastLine(string line)
        {
            return line.IndexOf("-ERR", StringComparison.InvariantCulture) == 0 || line.Trim() == ".";
        }
    }
}
