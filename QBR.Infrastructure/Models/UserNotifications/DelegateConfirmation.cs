using System;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;

namespace QBR.Infrastructure.Models.UserNotifications
{
    public class DelegateConfirmation : Confirmation
    {
        public Action ConfirmedAction { get; set; }

        public Action CancelAction { get; set; }
    }
}
