using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using QBR.AnalyzerManagerModule.Services;
using QBR.Infrastructure.CompositePresentationEvents;
using QBR.Infrastructure.Interfaces;
using QBR.Infrastructure.Models;
using QBR.Infrastructure.Models.Enums;
using QBR.Infrastructure.Models.EventArguments;

namespace QBR.AnalyzerManagerModule.ViewModels
{
    public class AnalyzerManagerViewModel : BindableObject
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IDispatcherService _dispatcherService;
        private readonly IUserNotificationService _userNotificationService;
        private readonly IUserEntryService _userEntryService;
        private readonly IAnalyzerConfigurationService _analyzerConfigurationService;

        private bool _showAnalyzerStatus = true;
        private StripType _stripType;
        private int _bankID;

        public AnalyzerManagerViewModel(AnalyzerManager analyzerManager, IEventAggregator eventAggregator,
            IDispatcherService dispatcherService, IUserNotificationService userNotificationService,
            IUserEntryService userEntryService, IAnalyzerConfigurationService analyzerConfigurationService)
            : this()
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<TestStartedEvent>().Subscribe(o => OnTestStarted());
            _eventAggregator.GetEvent<TestAbortedEvent>().Subscribe(o => OnTestAborted());
            _eventAggregator.GetEvent<TestCompletedEvent>().Subscribe(o => OnTestCompleted());
            _eventAggregator.GetEvent<AnalyzerSelectionChangedEvent>().Subscribe(o => OnAnalyzerSelectionChanged());

            analyzerManager.AnalyzerConnection += OnAnalyzerConnection;
            analyzerManager.InitializeConnectedAnanlyzers();

            _dispatcherService = dispatcherService;
            _userNotificationService = userNotificationService;
            _userEntryService = userEntryService;
            _userEntryService.UserEntryUpdated += OnUserEntryUpdated;

            _analyzerConfigurationService = analyzerConfigurationService;
            ClearCommand = new DelegateCommand(OnClear);

            for (int i = 0; i < _analyzerConfigurationService.GetAnalyzerCount(); i++)
            {
                AnalyzerViewModels.Add(new AnalyzerViewModel((AnalyzerID)(i+1), _eventAggregator));
            }
        }

        public AnalyzerManagerViewModel()
        {
            AnalyzerViewModels = new ObservableCollection<AnalyzerViewModel>();
        }

        public ObservableCollection<AnalyzerViewModel> AnalyzerViewModels { get; set; }

        private void OnAnalyzerConnection(object sender, AnalyzerConnectionEventArgs eventArgs)
        {
            if (eventArgs.IsConnected)
            {
                try
                {
                    var analyzerID = _analyzerConfigurationService.GetAnalyzerID(eventArgs.Analyzer.BuildInfo.SerialNumber);
                    var analyzerViewModel = AnalyzerViewModels.First(a => a.AnalyzerID == analyzerID);
                    analyzerViewModel.SetAnalyzer(eventArgs.Analyzer);
                }
                catch (Exception)
                {
                    _userNotificationService.Notify(new Notification()
                    {
                        Content =
                            string.Format("ERROR: \n{0} {1}", eventArgs.Analyzer.BuildInfo.SerialNumber,
                                " does not belong to this analyzer bank"),
                        Title = "Error"
                    });
                }
            }
            else
            {
                var analyzerViewModel = AnalyzerViewModels.FirstOrDefault(a => a.Analyer == eventArgs.Analyzer);
                if (analyzerViewModel == null)
                    return;
                analyzerViewModel.AnalyzerDisconnected();
            }
        }

        public bool ShowAnalyzerStatus
        {
            get { return _showAnalyzerStatus; }
            set { SetProperty(ref _showAnalyzerStatus, value, "ShowAnalyzerStatus"); }
        }

        public ICommand ClearCommand {get; private set;} 

        /// <summary>
        /// Called when [test started].
        /// </summary>
        private void OnTestStarted()
        {
            AnalyzerViewModels.ForEach(a => { if (a.IsSelected) a.StartTest(); });
        }

        private void OnTestCompleted()
        {
            //Deactivate the Analyzer Manager View
            //ShowAnalyzerStatus = false;
        }

        private void OnTestAborted()
        {
            AnalyzerViewModels.ForEach(a => { if (a.IsSelected) a.AbortTest(); });
        }

        private void OnClear()
        {
            AnalyzerViewModels.ForEach(a => a.IsSelected = false);
        }

        private void OnUserEntryUpdated(object sender, EventArgs eventArgs)
        {
            if (_stripType == _userEntryService.StripType && _bankID == _userEntryService.BankID && !_userEntryService.HasError)
                return;
            _stripType = _userEntryService.StripType;
            _bankID = _userEntryService.BankID;

            AnalyzerViewModels.ForEach(a => a.UpdateVialCase(_userEntryService.StripType, _userEntryService.BankID));
        }

        private void OnAnalyzerSelectionChanged()
        {
            _eventAggregator.GetEvent<AnalyzerReadyEvent>()
                .Publish(AnalyzerViewModels.Any(analyzerViewModel => analyzerViewModel.IsSelected));
        }

        //#region Demo Only Code
        //public InteractionRequest<Notification> NotificationRequest { get; private set; }

        //private void NotifyUser(AnalyzerStatusChangedEventArgs eventArgs)
        //{
        //    switch (eventArgs.NewStatus)
        //    {
        //        case AnalyzerStatus.AwaitStripInsertion:
        //            lock (_lock)
        //            {
        //                if (++_meterCount == 8)
        //                {
        //                    _dispatcherService.Dispatch(() =>
        //                        NotificationRequest.Raise(
        //                            new Notification()
        //                            {
        //                                Content =
        //                                    string.Format("Insert strip to meters"),
        //                                Title = "Insert Strip"
        //                            },
        //                            o => AnalyzerViewModels.ForEach(vm => ((IDemoAnalyzer) vm.Analyer).InsertStrip())));
        //                    _meterCount = 0;
        //                }
        //            }
        //            break;
        //        case AnalyzerStatus.AwaitSampleApplication:
        //            lock (_lock)
        //            {
        //                if (++_meterCount == 8)
        //                {
        //                    _dispatcherService.Dispatch(() =>
        //                        NotificationRequest.Raise(
        //                            new Notification()
        //                            {
        //                                Content =
        //                                    string.Format("Apply sample to meters"),
        //                                Title = "Apply Sample"
        //                            },
        //                            o => AnalyzerViewModels.ForEach(vm => ((IDemoAnalyzer)vm.Analyer).ApplySample())));
        //                    _meterCount = 0;
        //                }
        //            }
        //            break;
        //        case AnalyzerStatus.AwaitStripEjection:
        //            lock (_lock)
        //            {
        //                if (++_meterCount == 8)
        //                {
        //                    _dispatcherService.Dispatch(() =>
        //                        NotificationRequest.Raise(
        //                            new Notification()
        //                            {
        //                                Content =
        //                                    string.Format("Eject strip to meters"),
        //                                Title = "Eject strip"
        //                            },
        //                            o => AnalyzerViewModels.ForEach(vm => ((IDemoAnalyzer)vm.Analyer).EjectStrip())));
        //                    _meterCount = 0;
        //                }
        //            }
        //            break;
        //        case AnalyzerStatus.TestCompleted:
        //            //_waitForUserConfirm.WaitOne();
        //            //NotificationRequest.Raise(new Notification() { Content = string.Format("Insert strip to {0}", eventArgs.Analyzer.BuildInfo.SerialNumber) });
        //            break;
        //        case AnalyzerStatus.Failed:
        //            _dispatcherService.Dispatch(() =>
        //                _userNotificationService.Notify(new Notification()
        //                {
        //                    Content =
        //                        string.Format("ERROR: \n{0} {1}", eventArgs.SerialNumber, eventArgs.AnalyzerFailure),
        //                    Title = "Error"
        //                }));
        //            break;
        //        default:
        //            break;
        //    }
        //}
        //#endregion
    }
}
