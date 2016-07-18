using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Interactivity;
using Microsoft.Expression.Interactivity.Media;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using QBR.Infrastructure.CompositePresentationEvents;
using QBR.Infrastructure.Interfaces;
using QBR.Infrastructure.Models;
using QBR.Infrastructure.Models.Enums;
using QBR.Infrastructure.Models.EventArguments;

namespace QBR.TestManagerModule.Services
{
    public class TestProgressService : ITestProgressService
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ISoundPlayingService _soundPlayingService;
        private readonly IDispatcherService _dispatcherService;
        private readonly IUserNotificationService _userNotificationService;
        private readonly Dictionary<string, AnalyzerStatus> _analyzerStatuses; 

        private bool _isAwaitStripInsertionPlayed;
        private bool _isAwaitSampleApplicationPlayed;
        private bool _isAwaitStripEjectionPlayed;
        private bool _isTestSuccessfulPlayed;
        private int _waitToFinishCount;

        public TestProgressService(IEventAggregator eventAggregator, ISoundPlayingService soundPlayingService, IDispatcherService dispatcherService, IUserNotificationService userNotificationService)
        {
            //InitializeTestStages();
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<AnalyzerStatusChangedEvent>().Subscribe(OnAnalyzerStatusChanged);
            _eventAggregator.GetEvent<ErrorDetectedEvent>().Subscribe(OnErrorDetected);
            _soundPlayingService = soundPlayingService;
            _dispatcherService = dispatcherService;
            _userNotificationService = userNotificationService;
        }

        public void AbortTest()
        {
            _eventAggregator.GetEvent<TestAbortedEvent>().Publish(null);            
        }

        public void StartTest()
        {
            _eventAggregator.GetEvent<TestStartedEvent>().Publish(null);            
        }

        //private void InitializeTestStages()
        //{
        //    TestStages = new List<TestStage>();
        //    var testStages = Properties.Settings.Default.TestStages;
        //    foreach (var testStage in testStages)
        //    {
        //        TestStages.Add(new TestStage(testStage));
        //    }
        //}

        private void OnAnalyzerStatusChanged(AnalyzerStatusChangedEventArgs eventArgs)
        {
            switch (eventArgs.NewStatus)
            {
                case AnalyzerStatus.AwaitStripInsertion:
                    Interlocked.Increment(ref _waitToFinishCount);
                    break;
                case AnalyzerStatus.TestCompleted:
                    Interlocked.Decrement(ref _waitToFinishCount);
                    break;
            }

            UpdateTestProgress();

            _dispatcherService.Dispatch(() => PlaySound(eventArgs.NewStatus));
        }

        private void OnErrorDetected(ErrorDetectedEventArgs eventArgs)
        {
            Interlocked.Decrement(ref _waitToFinishCount);
            _dispatcherService.Dispatch(() =>
                _userNotificationService.Notify(new Notification()
                {
                    Content =
                        string.Format("ERROR: \n{0} {1}", eventArgs.BuildInfo.SerialNumber, eventArgs.AnalyzerFailure),
                    Title = "Error"
                }));

            UpdateTestProgress();

            _dispatcherService.Dispatch(() => PlaySound(AnalyzerStatus.Failed));
        }

        private void UpdateTestProgress()
        {
            if (_waitToFinishCount == 0)
            {
                _isAwaitStripInsertionPlayed = false;
                _isAwaitSampleApplicationPlayed = false;
                _isAwaitStripEjectionPlayed = false;
                _isTestSuccessfulPlayed = false;
                _eventAggregator.GetEvent<TestCompletedEvent>().Publish(null);
            }
        }

        private void PlaySound(AnalyzerStatus newStatus)
        {
            switch (newStatus)
            {
                case AnalyzerStatus.AwaitStripInsertion:
                    if (!_isAwaitStripInsertionPlayed)
                    {
                        _isAwaitStripInsertionPlayed = true;
                        _soundPlayingService.PlayAttentionSound();
                    }
                    break;
                case AnalyzerStatus.AwaitSampleApplication:
                    if (!_isAwaitSampleApplicationPlayed)
                    {
                        _isAwaitSampleApplicationPlayed = true;
                        _soundPlayingService.PlayAttentionSound();
                    }
                    break;
                case AnalyzerStatus.AwaitStripEjection:
                    if (!_isAwaitStripEjectionPlayed)
                    {
                        _isAwaitStripEjectionPlayed = true;
                        _soundPlayingService.PlayAttentionSound();
                    }
                    break;
                case AnalyzerStatus.SendingTransient:
                    if (!_isTestSuccessfulPlayed)
                    {
                        _isTestSuccessfulPlayed = true;
                        _soundPlayingService.PlaySuccessSound();
                    }
                    break;
                case AnalyzerStatus.Failed:
                    _soundPlayingService.PlayErrorSound();
                    break;
                default:
                    break;
            }
        }

        //private void OnTestStarted()
        //{
        //    TestStages.First().TestStatus = TestStatus.TestInProgress;
        //    _eventAggregator.GetEvent<TestContinuedEvent>().Publish(null);            
        //}

        //private void OnTestStageComplted()
        //{

        //}

        //public List<TestStage> TestStages { get; set; }
    }
}
