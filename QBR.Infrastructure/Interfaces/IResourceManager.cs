using System;

namespace QBR.Infrastructure.Interfaces
{
    public interface IResourceManager
    {
        void RegisterModuleResourceDictionary(Uri packUri);
    }
}