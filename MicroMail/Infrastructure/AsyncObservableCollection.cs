using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;

namespace MicroMail.Infrastructure
{
    public class AsyncObservableCollection<T> : ObservableCollection<T>
    {
        private readonly SynchronizationContext _syncContext = SynchronizationContext.Current;

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (SynchronizationContext.Current == _syncContext)
            {
                base.OnCollectionChanged(e);
            }
            else
            {
                _syncContext.Post(DispatchCollectionChanged, e);
            }
        }

        private void DispatchCollectionChanged(object e)
        {
            base.OnCollectionChanged((NotifyCollectionChangedEventArgs)e);
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (SynchronizationContext.Current == _syncContext)
            {
                base.OnPropertyChanged(e);
            }
            else
            {
                _syncContext.Post(DispatchPropertyChanged, e);
            }
        }

        private void DispatchPropertyChanged(object e)
        {
            base.OnPropertyChanged((PropertyChangedEventArgs) e);
        }

    }
}
