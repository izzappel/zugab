using System;
using System.Windows;
using System.Windows.Threading;

namespace ZuegerAdressbook.Service
{
    public class ApplicationDispatcher : IDispatcher
    {
        public void Dispatch(Delegate method)
        {
            UnderlyingDispatcher.BeginInvoke(method, DispatcherPriority.ContextIdle, null);
        }

        private Dispatcher UnderlyingDispatcher
        {
            get
            {
                if (Application.Current == null)
                {
                    throw new InvalidOperationException("You must call this method from within a running WPF application!");
                }

                if (Application.Current.Dispatcher == null)
                {
                    throw new InvalidOperationException("You must call this method from within a running WPF application with an active dispatcher!");
                }

                return Application.Current.Dispatcher;
            }
        }
    }
}
