using System;
using System.Collections.Generic;
using System.Media;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Logging;
using QBR.Infrastructure.Extensions;
using QBR.Infrastructure.Interfaces;
using QBR.Infrastructure.Models.EventArguments;
using QBR.Infrastructure.Models.UserNotifications;

namespace QBR.Shell.Services
{
    public class UserNotificationService : IUserNotificationService
    {
        private readonly Queue<Notification> _notifications;
        private readonly ILoggerFacade _logger;
        private bool _isNotifyingUser;
            
        public UserNotificationService(ILoggerFacade logger)
        {
            _logger = logger;

            _notifications = new Queue<Notification>();
        }

        public void Notify(Notification note)
        {
            _logger.Log(note.Content.ToString(), note.Title == "Warning" ? Category.Warn : Category.Exception, Priority.None);

            lock (_notifications)
            {
                if (!_isNotifyingUser)
                {
                    _isNotifyingUser = true;
                    Notifying.Raise(this, new UserNotificationEventArgs(note, note.GetType()));
                }
                else
                {
                    _notifications.Enqueue(note);                    
                }
            }
        }

        public void Resolve(Notification notification)
        {
            if (notification is DelegateConfirmation)
            {
                var confirmation = notification as DelegateConfirmation;
                if (confirmation.Confirmed)
                {
                    confirmation.ConfirmedAction.Invoke();
                }
                else
                {
                   confirmation.CancelAction.Invoke();
                }
            }

            lock (_notifications)
            {
                if (_notifications.Count == 0)
                {
                    _isNotifyingUser = false;
                }
                else
                {
                    Notifying.Raise(this, new UserNotificationEventArgs(_notifications.Dequeue()));
                }
            }
        }

        public event EventHandler<UserNotificationEventArgs> Notifying;
    }
}
