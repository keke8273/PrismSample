using System;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;

namespace QBR.Infrastructure.Models.EventArguments
{
    public class UserNotificationEventArgs : EventArgs
    {
        private readonly Type _notificationType;
        private readonly Notification _notification;

        public UserNotificationEventArgs(Notification notification, Type notificationType = null)
        {
            _notification = notification;
            _notificationType = notificationType;
        }

        public Type NotificationType { get { return _notificationType; } }

        public Notification Notification { get { return _notification; } }

    }
}
