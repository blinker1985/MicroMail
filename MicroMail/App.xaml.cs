using System.Windows;
using MicroMail.Infrastructure;
using MicroMail.Infrastructure.Messaging.Events;
using MicroMail.Windows;
using Ninject;
using Ninject.Modules;

namespace MicroMail
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private ApplicationWorker _worker;

        protected override void OnStartup(StartupEventArgs e)
        {
            var kernel = new StandardKernel(new INinjectModule[]{new DiModule()});

            RegisterTrgiggeredWindows(kernel.Get<WindowsAllocator>());

            _worker = kernel.Get<ApplicationWorker>();
            _worker.Start(Current);

            
            base.OnStartup(e);
        }

        private static void RegisterTrgiggeredWindows(WindowsAllocator allocator)
        {
            allocator.Register<MailWindow, ShowMailWindowEvent>();
            allocator.Register<AboutWindow>(PlainWindowEvents.ShowAboutWindow);
            allocator.Register<MailListWindow>(PlainWindowEvents.ShowMailListWindow);
            allocator.Register<SettingsWindow>(PlainWindowEvents.ShowSettingsWindow);
        }
    }
}
