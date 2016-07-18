using System;
using System.Threading;
using System.Windows.Threading;
using Microsoft.Practices.Prism.Logging;
using QBR.Infrastructure.Extensions;
using QBR.Infrastructure.Interfaces;
using QBR.Infrastructure.Models.DataTypes;
using QBR.Infrastructure.Models.Enums;
using QBR.Infrastructure.Models.EventArguments;

namespace QBR.Infrastructure.Models.Analyzers
{

    public class DummyAnalyzer : AnalyzerBase, IDemoAnalyzer
    {
        private static int _analyzerCount;
        private AutoResetEvent _waitForTestStart = new AutoResetEvent(false);
        private AutoResetEvent _waitForStripInsertion = new AutoResetEvent(false);
        private AutoResetEvent _waitForSampleApplication = new AutoResetEvent(false);
        private AutoResetEvent _waitForStripEjection = new AutoResetEvent(false);
        private Dispatcher _dispatcher;
        private bool _isErrorNotified;
        private Random _random = new Random(DateTime.Now.Millisecond);
        private ILoggerFacade _logger;
        private Transient _dummyTransient = new Transient
        {
            Result = new TransientResultLQC()
            {
                AccessionNumber = 2,
                TestType = (ushort)TestTypes.ProPTLQC,
                ValidData = EValidResult.ValidRecord,
                LQC = new LQCResult()
                {
                    BuildInformation = new ResultBuildInfo
                    {
                        SerialNumber = "Dummy" + ++_analyzerCount,
                        PartNumber = "TestPartNumber",
                        HWRelease = "TestHWRelease",
                        SWVersion = "TestSWRelease"
                    },
                    Sample = new SampleInfo
                    {
                        TypeFlag = 2,
                        DetectTime = 0,
                    },
                    FaultIdentifier = 0,
                    TestResult = new ExtResult
                    {
                        PassFail = 1,
                        OperatorID = "TestOID",
                        TestStripLotNumber = 1234,
                        TestStripExpiryDate = 0,
                        TestStripIndex = 12,
                        CCT = 0.1234f,
                        CT = 0.12345f,
                        INR = 0.123456f,
                    },
                    Transient = new TransientDetails
                    {
                        Minimum_Current = 0.1f,
                        Minimum_Time = 0.12f,
                        OBCValue = 0.123f,
                        PeakCurrent = 0.1234f,
                        Current1Point2MicroampRise = 0.12345f,
                        Current1Point3MicroampRise = 0.1234f,
                        Time1Point2MicroampRise = 0.123f,
                        Time1Point3MicroampRise = 0.12f
                    },
                    Vial = new VialDetails
                    {
                        ISI = 0.1f,
                        MNPT = 0.1f,
                        OBCLimit = 0.1f,
                        PFLimit = 0.1f
                    }
                },
            },
            TransientData =
                new TransientData() { TransientSize = 2, TransientStripCurrents = new[] { 0.1f, 0.12354f } }
        };

        public DummyAnalyzer(ILoggerFacade logger)
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            _logger = logger;
            BuildInfo = new BuildInfo()
            {
                SerialNumber = "Dummy" + _analyzerCount,
                PartNumber = "TestPartNumber",
                HWRelease = "TestHWRelease",
                SWVersion = "TestSWRelease"
            };
            AnalyzerType = AnalyzerTypes.PRO;
            AnalyzerStatus = AnalyzerStatus.Idle;

            ThreadPool.QueueUserWorkItem(state =>
            {
                while (true)
                {
                    _waitForTestStart.WaitOne();

                    while (AnalyzerStatus < AnalyzerStatus.TestCompleted)
                    {
                        AnalyzerStatus++;
                        AnalyzerStatusChanged.Raise(this, new AnalyzerStatusChangedEventArgs(AnalyzerStatus));

                        //if (AnalyzerStatus == AnalyzerStatus.AwaitStripInsertion)
                        //{
                        //    _waitForStripInsertion.WaitOne();
                        //}
                        //else if (AnalyzerStatus == AnalyzerStatus.AwaitSampleApplication)
                        //{
                        //    _waitForSampleApplication.WaitOne();
                        //}
                        //else if (AnalyzerStatus == AnalyzerStatus.AwaitStripEjection)
                        //{
                        //    _waitForStripEjection.WaitOne();
                        //}
                        if (AnalyzerStatus == AnalyzerStatus.SendingTransient)
                        {
                            Thread.Sleep(2000);
                            _dispatcher.BeginInvoke(
                                new Action(
                                    () => TransientArrived.Raise(this, new TransientArrivedEventArgs(_dummyTransient)))
                                );
                        }
                        else if (AnalyzerStatus == AnalyzerStatus.Heating)
                        {
                            if (!_isErrorNotified && _random.Next(8) == 7)
                            {
                                _isErrorNotified = true;
                                AnalyzerStatus = AnalyzerStatus.Failed;
                                ErrorDetected.Raise(this, new ErrorDetectedEventArgs(BuildInfo, AnalyzerFailure.HeaterOutOfRange));
                            }
                        }
                        else
                        {
                            Thread.Sleep(2000);
                        }
                    }
                }
            });
        }

        public override void Initialize()
        {
            throw new NotImplementedException();
        }

        public override void StartTest()
        {
            _logger.Log(string.Format("Starting analyzer {0}", BuildInfo.SerialNumber), Category.Info, Priority.Low);
            _isErrorNotified = false;
            AnalyzerStatus = AnalyzerStatus.Idle;
            _waitForTestStart.Set();
        }

        public override void AbortTest()
        {
            if (AnalyzerStatus == AnalyzerStatus.TestCompleted || AnalyzerStatus == AnalyzerStatus.Failed)
                return;

            _logger.Log(string.Format("Aborting analyzer {0}", BuildInfo.SerialNumber), Category.Info, Priority.Low);
            AnalyzerStatus = AnalyzerStatus.TestCompleted;
            AnalyzerStatusChanged.Raise(this, new AnalyzerStatusChangedEventArgs(AnalyzerStatus));
        }

        public override void GetLatestTransientRecord()
        {
            throw new NotImplementedException();
        }

        protected override void HandleAppLayerEvent(object p)
        {
            throw new NotImplementedException();
        }

        public override event EventHandler<AnalyzerStatusChangedEventArgs> AnalyzerStatusChanged;
        public override event EventHandler<TransientArrivedEventArgs> TransientArrived;
        public override event EventHandler<ErrorDetectedEventArgs> ErrorDetected;

        public void InsertStrip()
        {
            _waitForStripInsertion.Set();
        }

        public void ApplySample()
        {
            _waitForSampleApplication.Set();
        }

        public void EjectStrip()
        {
            _waitForStripEjection.Set();
        }
    }
}
