using System;
using System.Collections.Specialized;
using QBR.Infrastructure.Models.Enums;

namespace QBR.Infrastructure.Interfaces
{
    public interface IUserEntryService
    {
        Int32  TestID { get; set; }
        Int32  BankID { get; set; }
        string OperatorID { get; set; }
        string OutputDirectory { get; set; }
        string BatchNumber { get; set; }
        StringCollection RecentOutputDirectories { get; set; }
        StripType StripType { get; set; }
        string TestTarget { get; set; }
        double TargetINR { get; set; }
        double TargetHCT { get; set; }
        bool HasError { get; set; }
        string Comments { get; set; }

        bool IsAllDataCollected();
        
        event EventHandler UserEntryUpdated;
    }
}