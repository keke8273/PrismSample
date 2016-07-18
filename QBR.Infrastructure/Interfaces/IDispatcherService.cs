using System;

namespace QBR.Infrastructure.Interfaces
{
    public interface IDispatcherService
    {
        void Dispatch(Action action);

        void Dispatch(Delegate method, params object[] args);
    }
}
