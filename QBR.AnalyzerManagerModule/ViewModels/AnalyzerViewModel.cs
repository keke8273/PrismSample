using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using QBR.AnalyzerManagerModule.Models.Notifications;
using QBR.Infrastructure.CompositePresentationEvents;
using QBR.Infrastructure.Interfaces;
using QBR.Infrastructure.Models;
using QBR.Infrastructure.Models.Enums;
using QBR.Infrastructure.Models.EventArguments;

namespace QBR.AnalyzerManagerModule.ViewModels
{
    public class AnalyzerViewModel : BindableObject
    {
        private IAnalyzer _analyzer;
        private readonly IEventAggregator _eventAggregator;

        private AnalyzerID _analyzerID;
        private string _analyzerName = string.Empty;
        private string _vialCaseID = string.Empty;
        private AnalyzerStatus _analyzerStatus = AnalyzerStatus.Unknown;
        private AnalyzerTypes _analyzerType;
        private AnalyzerFailure _analyzerFailure = AnalyzerFailure.NoFailure;
        private bool _isSelected;
        private readonly InteractionRequest<TestResultNotification> _showTestResultRequest;

        //default constructor required for design time resource.
        public AnalyzerViewModel()
        {
        }

        public AnalyzerViewModel(AnalyzerID analyzerID, IEventAggregator eventAggregator)
        {
            _analyzerID = analyzerID;
            _analyzerName = analyzerID.ToString();
            _analyzerStatus = AnalyzerStatus.Disconnected;

            _showTestResultRequest = new InteractionRequest<TestResultNotification>();

            _eventAggregator = eventAggregator;

            SelectCommand = new DelegateCommand(SelectAnalyzer, CanSelectAnalyzer);
            RestartTestCommand = new DelegateCommand(StartTest);
            IsSelected = false;
        }

        public string AnalyzerName
        {
            get { return _analyzerName; }
            set { SetProperty(ref _analyzerName, value, "AnalyzerName"); }
        }

        public AnalyzerID AnalyzerID
        {
            get { return _analyzerID; }
            set { SetProperty(ref _analyzerID, value, "AnalyzerID"); } 
        }

        public AnalyzerStatus AnalyzerStatus
        {
            get { return _analyzerStatus; }
            set { SetProperty(ref _analyzerStatus, value, "AnalyzerStatus"); } 
        }

        public AnalyzerTypes AnalyzerType
        {
            get { return _analyzerType; }
            set { SetProperty(ref _analyzerType, value, "AnalyzerType"); } 
        }

        public AnalyzerFailure AnalyzerFailure
        {
            get { return _analyzerFailure; }
            set { SetProperty(ref _analyzerFailure, value, "AnalyzerFailure"); }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                SetProperty(ref _isSelected, value, "IsSelected");
                _eventAggregator.GetEvent<AnalyzerSelectionChangedEvent>().Publish(null);
            }
        }

        public string VialCaseID
        {
            get { return _vialCaseID; }
            set { SetProperty(ref _vialCaseID, value, "VialCaseID"); }        
        }

        public IInteractionRequest ShowTestResultRequest
        {
            get { return _showTestResultRequest; } 
        }

        public IAnalyzer Analyer { get { return _analyzer; } }

        public ICommand SelectCommand { get; private set; }

        public ICommand RestartTestCommand{ get; private set;}

        public void UpdateVialCase(StripType stripType, int bankID)
        {
            switch (stripType)
            {
                case StripType.Proteus:
                    if (bankID <= 2)
                    {
                        VialCaseID = string.Format("{0}{1}", 'M', ((int)AnalyzerID + 1)/2 + (bankID - 1)*4);
                    }
                    else if (bankID < 5)
                    {
                        VialCaseID = string.Format("{0}{1}", 'O', ((int)AnalyzerID + 1)/2 + (bankID - 3)*4);
                    }
                    else
                    {
                        VialCaseID = string.Empty;
                    }
                    break;
                default:
                    VialCaseID = string.Empty;
                    break;
            }
        }

        public void SetAnalyzer(IAnalyzer analyzer)
        {
            _analyzer = analyzer;
            _analyzer.AnalyzerStatusChanged += OnAnalyzerStatusChanged;
            _analyzer.TransientArrived += OnTransientArrived;
            _analyzer.ErrorDetected += OnErrorDetected;

            AnalyzerName = string.Format("{0} ({1})",AnalyzerID, _analyzer.BuildInfo.SerialNumber);
            AnalyzerType = analyzer.AnalyzerType;
            AnalyzerStatus = analyzer.AnalyzerStatus;
            IsSelected = true;
        }

        public void StartTest()
        {
            AnalyzerFailure = AnalyzerFailure.NoFailure;
            _analyzer.StartTest();
        }

        public void AbortTest()
        {
            _analyzer.AbortTest();
        }

        public void AnalyzerDisconnected()
        {
            _analyzer.AnalyzerStatusChanged -= OnAnalyzerStatusChanged;
            _analyzer.TransientArrived -= OnTransientArrived;
            _analyzer.ErrorDetected -= OnErrorDetected;
            _analyzer = null;

            AnalyzerType = AnalyzerTypes.Unknown;
            AnalyzerStatus = AnalyzerStatus.Disconnected;
            IsSelected = false;
        }

        private void OnAnalyzerStatusChanged(object sender, AnalyzerStatusChangedEventArgs eventArgs)
        {
            AnalyzerStatus = eventArgs.NewStatus;
            _eventAggregator.GetEvent<AnalyzerStatusChangedEvent>().Publish(eventArgs);
        }

        private void OnTransientArrived(object sender, TransientArrivedEventArgs eventArgs)
        {
            _showTestResultRequest.Raise(new TestResultNotification()
            {
                ClotTime = 1.1,
                PartialFill = 2.2,
                DoulbeFill = 3.3,
                OBCValue = 4.4,
                MinimumCurrent = 5.5
            });

            eventArgs.VialCaseId = VialCaseID;
            _eventAggregator.GetEvent<TransientArrivedEvent>().Publish(eventArgs);
        }

        private void OnErrorDetected(object sender, ErrorDetectedEventArgs eventArgs)
        {
            AnalyzerStatus = AnalyzerStatus.Failed;
            AnalyzerFailure = eventArgs.AnalyzerFailure;

            eventArgs.VialCaseId = VialCaseID;
            _eventAggregator.GetEvent<ErrorDetectedEvent>().Publish(eventArgs);
        }

        private void SelectAnalyzer()
        {
            IsSelected = !IsSelected;
        }

        private bool CanSelectAnalyzer()
        {
            return AnalyzerStatus == AnalyzerStatus.Idle || AnalyzerStatus == AnalyzerStatus.TestCompleted ||
                   AnalyzerStatus == AnalyzerStatus.Failed;
        }
    }
}
