using System;
using MicroMail.Properties;

namespace MicroMail.Infrastructure
{
    static public class Logger
    {
        public static void Debug(this object _this, string message)
        {
            Console.WriteLine(Resources.LoggerDebugTemplate, DateTime.Now.ToString("T"), _this.GetType().FullName, message);
        }
    }
}
