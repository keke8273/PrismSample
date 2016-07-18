using System;

namespace QBR.Infrastructure.Models.EventArguments
{
    public class TestStatusChangedEventArgs : EventArgs
    {
        private readonly TestStatus _currentStatus;

        public TestStatusChangedEventArgs(TestStatus currentStatus)
        {
            _currentStatus = currentStatus;
        }

        public TestStatus CurrentStatus { get { return _currentStatus; } }
    }
}
