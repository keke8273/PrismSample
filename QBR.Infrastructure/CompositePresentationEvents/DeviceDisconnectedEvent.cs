﻿using Microsoft.Practices.Prism.Events;
using QBR.Infrastructure.Models.EventArguments;

namespace QBR.Infrastructure.CompositePresentationEvents
{
    public class DeviceDisconnectedEvent : CompositePresentationEvent<AnalyzerConnectionEventArgs>
    {
    }
}
