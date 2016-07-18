using Microsoft.Practices.Prism.Events;

namespace QBR.Infrastructure.CompositePresentationEvents
{
    public enum CloseSplashSource
    {
        BootStrapper,
        User,
        SoftwareUpgrade
    }

    public class CloseSplashEvent : CompositePresentationEvent<CloseSplashSource>
    {
    }
}
