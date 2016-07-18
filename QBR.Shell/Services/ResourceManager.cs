using System;
using System.Windows;
using QBR.Infrastructure.Interfaces;

namespace QBR.Shell.Services
{
    public class ResourceManager : IResourceManager
    {
        public void RegisterModuleResourceDictionary(Uri packUri)
        {
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
            {
                Source = packUri
            });
        }
    }
}
