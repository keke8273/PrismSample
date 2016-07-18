using System;

namespace QBR.Infrastructure.Models.EventArguments
{
    public class TestProgressChangedEventArgs : EventArgs
    {
        private readonly double _progress;

        public TestProgressChangedEventArgs(double progress)
        {
            _progress = progress;
        }

        public double Progress { get { return _progress; } }
    }
}
