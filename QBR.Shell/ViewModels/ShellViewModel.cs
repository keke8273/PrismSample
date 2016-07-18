using System;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Logging;
using QBR.Infrastructure.CompositePresentationEvents;
using QBR.Infrastructure.Interfaces;
using QBR.Infrastructure.Models;
using QBR.Infrastructure.Models.EventArguments;

namespace QBR.Shell.ViewModels
{
    public class ShellViewModel : BindableObject
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ILoggerFacade _logger;
        private readonly IUserNotificationService _userNotificationService;
        private readonly InteractionRequest<Notification> _errorNotificationRequest;

        public ShellViewModel(IEventAggregator eventAggregator, ILoggerFacade logger, IUserNotificationService userNotificationService)
        {
            _eventAggregator = eventAggregator;
            _logger = logger;
            _userNotificationService = userNotificationService;
            _userNotificationService.Notifying += OnNotifying;

            _errorNotificationRequest = new InteractionRequest<Notification>();
           
            ClosedCommand = new DelegateCommand(OnClosed);
        }

        #region Properties
        public string ApplicationNameAndVersion { get; private set; }

        public ICommand ClosedCommand { get; private set; }

        public InteractionRequest<Notification> ErrorNotificationRequest
        {
            get { return _errorNotificationRequest; }
        }

        #endregion

        #region Functions
        private void OnClosed()
        {
            _logger.Log("Shutting Down Application", Category.Info, Priority.None);
            _eventAggregator.GetEvent<CloseApplicationEvent>().Publish(null);
        }

        private void OnNotifying(object sender, UserNotificationEventArgs eventArgs)
        {
            _errorNotificationRequest.Raise(eventArgs.Notification, o => _userNotificationService.Resolve(eventArgs.Notification));
        }
        #endregion
    }
}
