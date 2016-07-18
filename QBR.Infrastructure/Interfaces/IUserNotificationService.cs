using System;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using QBR.Infrastructure.Models.EventArguments;

namespace QBR.Infrastructure.Interfaces
{
    public interface IUserNotificationService
    {
        void Notify(Notification note);

        void Resolve(Notification notification);
        
        event EventHandler<UserNotificationEventArgs> Notifying;
    }
}