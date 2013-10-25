using System;

namespace MicroMail.Services.Pop3.Commands
{
    abstract class Pop3SingleLineCommand<T> : ServiceCommandBase<T> where T : ResponseBase, new()
    {
        protected Pop3SingleLineCommand(Action<T> callback) : base(callback)
        {
        }

        protected override bool IsLastLine(string line)
        {
            return true;
        }
    }
}
