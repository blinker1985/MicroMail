using System;

namespace MicroMail.Infrastructure.Messaging
{
    class Callback<T> : IExecutable where T : class 
    {
        public Action<T> InnerAction { get; set; }

        public void Execute(object parameter)
        {
            InnerAction.Invoke(parameter as T);
        }
    }
}
