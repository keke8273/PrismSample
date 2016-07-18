using System;
using System.Windows.Threading;
using QBR.Infrastructure.Interfaces;

namespace QBR.Shell.Services
{
    class DispatcherService : IDispatcherService
    {
        private readonly Dispatcher _dispatcher;

        public DispatcherService(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public void Dispatch(Action action)
        {
            _dispatcher.BeginInvoke(action);
        }

        public void Dispatch(Delegate method, params object[] args)
        {
            _dispatcher.BeginInvoke(method, args);
        }
    }
}
