using System;
using System.Collections.Generic;
using MicroMail.Infrastructure;
using System.Linq;

namespace MicroMail.Services.Pop3.Responses
{
    class Pop3ListResponse : Pop3MultilineResponse
    {
        public Dictionary<string, string> List { get; private set; }

        protected override void Complete()
        {
            var message = ResponseDetails.Body;
            List = new Dictionary<string, string>();
            var l = message.Split(new []{"\r\n"}, StringSplitOptions.RemoveEmptyEntries).ToList();
            l = l.GetRange(1, l.Count - 2);

            foreach (var keyValue in l.Select(s => s.Split(' ')))
            {
                List.Add(keyValue[0], keyValue[1]);
            }

            this.Debug(message);
        }
    }
}
