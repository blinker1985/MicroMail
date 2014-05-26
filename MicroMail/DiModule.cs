using MicroMail.Infrastructure;
using MicroMail.Infrastructure.MailStorage;
using MicroMail.Infrastructure.Messaging;
using MicroMail.Models;
using Ninject.Modules;

namespace MicroMail
{
    class DiModule : NinjectModule
    {
        public override void Load()
        {
            Bind<AccountsSettingsModel>().ToSelf().InSingletonScope();
            Bind<ApplicationSettingsModel>().ToSelf().InSingletonScope();
            Bind<EventBus>().ToSelf().InSingletonScope();
            Bind<AsyncObservableCollection<EmailGroupModel>>().ToSelf().InSingletonScope();
            Bind<ApplicationWorker>().ToSelf();
            Bind<WindowsAllocator>().ToSelf();
            Bind<IMailStorage>().To<LocalMailStorage>().InSingletonScope();
            Bind<OutgoingMailServiceResolver>().ToSelf();
        }
    }
}
