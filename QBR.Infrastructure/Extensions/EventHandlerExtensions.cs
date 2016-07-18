using System;

namespace QBR.Infrastructure.Extensions
{
    public static class EventHandlerExtensions
    {
        public static void Raise(this EventHandler handler, object sender)
        {
            if (handler != null)
            {
                handler(sender, EventArgs.Empty);
            }
        }

        public static void Raise<TA>(this EventHandler<TA> handler, object sender, TA args) where TA : EventArgs
        {
            if (handler != null)
            {
                handler(sender, args);
            }
        }
    }
}
