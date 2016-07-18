using System;
using QBR.Infrastructure.Interfaces;

namespace QBR.RecordManagerModule.Services
{
    public class TimeStampService : ITimeStampService
    {
        public DateTime TimeStamp {get { return DateTime.Now;} }
    }
}
