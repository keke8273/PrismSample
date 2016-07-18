using System;
using System.Collections.Generic;
using System.Linq;
using DataLinkLayer.IO;
using DataLinkLayer.IO.CommsCntrl;
using DataLinkLayer.Utils;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using QBR.Infrastructure.Extensions;
using QBR.Infrastructure.Interfaces;
using QBR.Infrastructure.Models.Analyzers;
using QBR.Infrastructure.Models.Enums;
using QBR.Infrastructure.Models.EventArguments;

namespace QBR.AnalyzerManagerModule.Services
{
    public class AnalyzerManager : IAnalyzerConnectionService, ISubscriber
    {
        private const int PIDIndex = 21;
        private const int PIDLength = 4;

        private readonly IEventAggregator _eventAggregator;
        private readonly ILoggerFacade _logger;
        private readonly IAnalyzerConfigurationService _analyzerConfigurationService;
        private readonly ICommsInterface _physicalLayerInterface;
        private readonly IDispatcherService _dispatcherService;

        public AnalyzerManager()
        {
            Analyzers = new List<IAnalyzer>();
        }

        public AnalyzerManager(IEventAggregator eventAggregator, ILoggerFacade logger, IAnalyzerConfigurationService analyzerConfigurationService, ICommsInterface physicalLayerInterface, IDispatcherService dispatcherService) :
            this()
        {
            _eventAggregator = eventAggregator;

            _logger = logger;

            _analyzerConfigurationService = analyzerConfigurationService;

            _physicalLayerInterface = physicalLayerInterface;

            _dispatcherService = dispatcherService;
        }

        /// <summary>
        /// Connects the existing devices.
        /// </summary>
        public void InitializeConnectedAnanlyzers()
        {
            var connectionStrings = _physicalLayerInterface.GetAvailablePorts();

            foreach (var connectionString in connectionStrings)
            {
                var port = _physicalLayerInterface.GetPort(connectionString);
                if (port.IsOpen) break;

                var analyzer = CreateNewAnalyzer(port, connectionString);
                Analyzers.Add(analyzer);
                analyzer.Initialize();
            }

            //Start to listen to future connection/disconnection event
            _physicalLayerInterface.RegisterSubscriber(this as ISubscriber);
        }

        /// <summary>
        /// Called when [notification].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="EventArgs"/> instance containing the event data.</param>
        void ISubscriber.OnNotification(object sender, EventArgs args)
        {
            OnAnalyzerConnection(sender, args);
        }

        private IAnalyzer CreateNewAnalyzer(IIOPort usbPort, string connectionString)
        {
            var analyzerType = GetAnalyzerType(connectionString);

            if (analyzerType != null)
            {
                var analyzer =  (IAnalyzer)Activator.CreateInstance(analyzerType, new object[] { usbPort, connectionString, new CommsWorker(usbPort), _dispatcherService});
                analyzer.AnalyzerStatusChanged += OnAnalyzerStatusChanged;
                return analyzer;
            }

            return null;
        }

        /// <summary>
        /// Gets the type of the analyzer.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>
        /// The analyzer type
        /// </returns>
        private Type GetAnalyzerType(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                return null;

            var productID = connectionString.Substring(PIDIndex, PIDLength).ToLower();
            switch (productID)
            {
                case "000a":
                    return typeof(ProteusAnalyzer);
                case "0014":
                    //return typeof(RubixController);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Simple event handler for when connection event occurs at the physical layer
        /// </summary>
        /// <param name="sender">The physical layer object the event occurred on</param>
        /// <param name="e">event arguments that contain more details of the connection event</param>
        private void OnAnalyzerConnection(object sender, EventArgs e)
        {
            var ciArgs = e as CommsInterfaceEventArgs;

            if (ciArgs != null)
            {
                //Find the device controller
                var analyzer = Analyzers.FirstOrDefault(dc => dc.ConnectionString == ciArgs.ConnectionString);

                //Connection Events
                if (ciArgs.DeviceConnected)
                {
                    if (analyzer == null)
                    {
                        var port = _physicalLayerInterface.GetPort(ciArgs.ConnectionString);
                        if (port.IsOpen) return;

                        analyzer = CreateNewAnalyzer(port, ciArgs.ConnectionString);
                        Analyzers.Add(analyzer);
                        analyzer.Initialize();
                    }
                }
                //Disconnection Events
                else
                {
                    Analyzers.Remove(analyzer);
                    AnalyzerConnection.Raise(this, new AnalyzerConnectionEventArgs(false, analyzer));
                }
            }
        }

        private void OnAnalyzerStatusChanged(object sender, AnalyzerStatusChangedEventArgs eventArgs)
        {
            var analyzer = sender as IAnalyzer;

            if (eventArgs.NewStatus == AnalyzerStatus.Idle)
            {
                analyzer.AnalyzerStatusChanged -= OnAnalyzerStatusChanged;
                AnalyzerConnection.Raise(this, new AnalyzerConnectionEventArgs(true, analyzer));
            }
        }

        public List<IAnalyzer> Analyzers { get; set; }

        public event EventHandler<AnalyzerConnectionEventArgs> AnalyzerConnection;

    }
}
