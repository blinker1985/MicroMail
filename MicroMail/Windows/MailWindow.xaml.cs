using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Navigation;
using MicroMail.Infrastructure.Messaging;
using MicroMail.Infrastructure.Messaging.Events;
using MicroMail.Models;

namespace MicroMail.Windows
{
    /// <summary>
    /// Interaction logic for MailWindow.xaml
    /// </summary>
    public partial class MailWindow : IEventWindow, ISingularWindow
    {
        private readonly EventBus _eventBus;
        private Stream _contentStream;

        public MailWindow(EventBus eventBus)
        {
            _eventBus = eventBus;
            InitializeComponent();

            Unloaded += UnloadedHandler;
            MessageWebView.Navigated += MessageWebViewOnNavigated;
        }

        private EmailModel _email;

        public EmailModel Email {
            get { return _email; }
            set 
            {
                if (_email != null)
                {
                    Email.PropertyChanged -= EmailPropertyChangedHandler;
                }

                _email = value;
                
                if (_email != null)
                {
                    Email.PropertyChanged += EmailPropertyChangedHandler;
                }

                UpdateViewData();
            }
        }

        public string SingularId {
            get { return Email != null ? Email.ServiceId + "_" + Email.Id : string.Empty; }
        }

        private void UnloadedHandler(object sender, RoutedEventArgs routedEventArgs)
        {
            Unloaded -= UnloadedHandler;
            MessageWebView.Navigated -= MessageWebViewOnNavigated;
            Email.PropertyChanged -= EmailPropertyChangedHandler;

            if (_contentStream != null)
            {
                _contentStream.Dispose();
            }
        }

        private void EmailPropertyChangedHandler(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            RenderEmail();
        }

        private void UpdateViewData()
        {
            if (Email.Body == null)
            {
                _eventBus.Trigger(new FetchMailBodyEvent(Email));
            }
            else
            {
                RenderEmail();
            }

            DataContext = null;
            DataContext = this;
        }

        private void RenderEmail()
        {
            if (string.IsNullOrEmpty(Email.Charset) || string.IsNullOrEmpty(Email.Body))
            {
                return;
            }
            var bytes = Encoding.GetEncoding(Email.Charset,
                                          new EncoderExceptionFallback(),
                                          new DecoderExceptionFallback())
                                .GetBytes(Email.Body);
            _contentStream = new MemoryStream(bytes);

            MessageWebView.NavigateToStream(_contentStream);
        }

        private void MessageWebViewOnNavigated(object sender, NavigationEventArgs navigationEventArgs)
        {
            if (_contentStream != null)
            {
                _contentStream.Dispose();
            }
        }

        public void ReceiveEvent(object triggeredEvent)
        {
            var e = triggeredEvent as ShowMailWindowEvent;
            if (e != null)
            {
                Email = e.Email;
            }
        }
    }
}
