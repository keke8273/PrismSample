using System;
using QBR.Infrastructure.Models;
using QBR.Infrastructure.Models.EventArguments;

namespace QBR.TestManagerModule.ViewModels
{
    public class TestStageViewModel : BindableObject
    {
        private double _testProgress;
        private TestStatus _testStatus;

        public TestStageViewModel(TestStage testStage)
        {
            StageName = testStage.StageName;
            TestStatus = testStage.TestStatus;

            testStage.TestStepChanged += OnTestStepChanged;
            testStage.TestStatusChanged += OnTestStatusChanged;
        }

        private void OnTestStepChanged(object sender, TestProgressChangedEventArgs eventArgs)
        {
            TestProgress = eventArgs.Progress;
        }

        private void OnTestStatusChanged(object sender, TestStatusChangedEventArgs eventArgs)
        {
            TestStatus = eventArgs.CurrentStatus;
        }

        public string StageName { get; private set; }

        public double TestProgress
        {
            get { return _testProgress; }
            set { SetProperty(ref _testProgress, value, "TestProgress"); }
        }

        public TestStatus TestStatus
        {
            get { return _testStatus; }
            set { SetProperty(ref _testStatus, value, "TestStatus"); }
        }
    }
}
