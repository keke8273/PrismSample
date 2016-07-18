using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using QBR.Infrastructure.CompositePresentationEvents;
using QBR.Infrastructure.Interfaces;
using QBR.Infrastructure.Models;
using QBR.Infrastructure.Models.EventArguments;
using QBR.Infrastructure.Models.UserNotifications;

namespace QBR.TestManagerModule.ViewModels
{
    public class TestProgressViewModel : BindableObject
    {
        private readonly ITestProgressService _testProgressService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IUserEntryService _userEntryService;
        private readonly IUserNotificationService _userNotificationService;

        //private ObservableCollection<TestStageViewModel> _testProgress { get; set; }
        private bool _showTestProgress = true;
        private bool _analyzerReady = false;

        public TestProgressViewModel()
        {
            //_testProgress = new ObservableCollection<TestStageViewModel>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestProgressViewModel" /> class.
        /// </summary>
        /// <param name="testProgressService">The test progress service.</param>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name="userEntryService">The user entry service.</param>
        /// <param name="userNotificationService">The user notification service.</param>
        public TestProgressViewModel(ITestProgressService testProgressService, IEventAggregator eventAggregator, IUserEntryService userEntryService, IUserNotificationService userNotificationService)
            :this()
        {
            _testProgressService = testProgressService;
            //foreach (var testStage in _testProgressService.TestStages)
            //{
            //    _testProgress.Add(new TestStageViewModel(testStage));
            //}

            _userEntryService = userEntryService;
            _userEntryService.UserEntryUpdated += OnUserEntryUpdated;

            _userNotificationService = userNotificationService;

            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<TestCompletedEvent>().Subscribe(OnTestCompleted);
            _eventAggregator.GetEvent<AnalyzerReadyEvent>().Subscribe(OnAnalyzerReady);

            StartTestCommand = new DelegateCommand(StartTest, CanStartTest);
            AbortTestCommand = new DelegateCommand(AbortTest, CanAbortTest);
        }

        //public ObservableCollection<TestStageViewModel> TestProgress
        //{
        //    get { return _testProgress; }
        //}

        public bool ShowTestProgress
        {
            get { return _showTestProgress;} 
            set { SetProperty(ref _showTestProgress, value, "ShowTestProgress");}
        }

        public bool TestInProgress { get; set; }

        public bool AnalyzerReady
        {
            get { return _analyzerReady; }
        }

        public ICommand StartTestCommand { get; private set; }

        public ICommand AbortTestCommand { get; private set; }

        private bool CanStartTest()
        {
            return _userEntryService.IsAllDataCollected() && (!_userEntryService.HasError) && (!TestInProgress) &&
                   AnalyzerReady;
        }

        private void StartTest()
        {
            TestInProgress = true;
            (StartTestCommand as DelegateCommand).RaiseCanExecuteChanged();
            (AbortTestCommand as DelegateCommand).RaiseCanExecuteChanged();
            _testProgressService.StartTest();
            //Navigate to DataReferenceView
            //_regionManager.RequestNavigate(RegionNames.UserDataRegion, new Uri(typeof(DataReferenceViewModel).FullName, UriKind.RelativeOrAbsolute));
        }

        private bool CanAbortTest()
        {
            return TestInProgress;
        }

        private void AbortTest()
        {
            _userNotificationService.Notify(new DelegateConfirmation()
            {
                Content = "Are you sure you want to abort the test?",
                Title = "Abort Test",
                ConfirmedAction = ConfirmedAbortTest,
                CancelAction = ()=>{},
            });
        }

        private void ConfirmedAbortTest()
        {
            _testProgressService.AbortTest();
        }

        private void OnTestCompleted(object obj)
        {
            TestInProgress = false;
            (StartTestCommand as DelegateCommand).RaiseCanExecuteChanged();
            (AbortTestCommand as DelegateCommand).RaiseCanExecuteChanged();
        }

        private void OnUserEntryUpdated(object sender, EventArgs eventArgs)
        {
            (StartTestCommand as DelegateCommand).RaiseCanExecuteChanged();
        }

        private void OnAnalyzerReady(bool analyzerReady)
        {
            _analyzerReady = analyzerReady;
            (StartTestCommand as DelegateCommand).RaiseCanExecuteChanged();
        }
    }
}
