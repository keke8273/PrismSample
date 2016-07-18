using System;

namespace QBR.Infrastructure.Interfaces
{
    public interface ISoftwareUpgradeService
    {
        void CheckForUpdate();

        Version CurrentVersion { get; }
    }
}