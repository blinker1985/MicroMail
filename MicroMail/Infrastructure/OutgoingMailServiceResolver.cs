using System;
using MicroMail.Models;
using MicroMail.Services;
using MicroMail.Services.Imap;
using Ninject;
using Ninject.Syntax;
using MicroMail.Services.Pop3;

namespace MicroMail.Infrastructure
{
    public class OutgoingMailServiceResolver
    {
        private readonly IResolutionRoot _injector;

        public OutgoingMailServiceResolver(IResolutionRoot injector)
        {
            _injector = injector;
        }

        public IFetchMailService Resolve(ProtocolTypeEnum protocolType)
        {
            Type t = null;
            switch (protocolType)
            {
                case ProtocolTypeEnum.Imap:
                    t = typeof(ImapService);
                    break;
                case ProtocolTypeEnum.Pop3:
                    t = typeof(Pop3Service);
                    break;
            }

            return t != null 
                ? _injector.Get(t) as IFetchMailService 
                : null;
        }
    }
}
