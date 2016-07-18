using System;

namespace QBR.Utilities
{
    public static class EventHandlerExtension
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
