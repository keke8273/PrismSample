using System;
using QBR.Infrastructure.Extensions;
using QBR.Infrastructure.Models.EventArguments;

namespace QBR.Infrastructure.Models
{
    public class TestStage
    {
        public const int StepCount = 56;
        private readonly string _testStageName;
        private int _testProgress;
        private TestStatus _testStatus = TestStatus.NotStarted;

        public TestStage(string testStageName)
        {
            _testStageName = testStageName;
        }

        public string StageName { get { return _testStageName; }}

        public int TestProgress
        {
            get { return _testProgress; }
            set
            {
                _testProgress = value;
                TestStepChanged.Raise(this, new TestProgressChangedEventArgs(_testProgress / (double)StepCount));
            }
        }

        public TestStatus TestStatus
        {
            get { return _testStatus; }
            set
            {
                _testStatus = value;
                TestStatusChanged.Raise(this, new TestStatusChangedEventArgs(_testStatus));
            }
        }
        
        public void Reset()
        {
            TestStatus = TestStatus.NotStarted;
            TestProgress = 0;
        }

        public event EventHandler<TestProgressChangedEventArgs> TestStepChanged;
        public event EventHandler<TestStatusChangedEventArgs> TestStatusChanged;
    }
}
