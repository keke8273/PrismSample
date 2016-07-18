using System;
using System.Collections.Generic;
using QBR.Infrastructure.Interfaces;
using QBR.Infrastructure.Models.Enums;

namespace QBR.AnalyzerManagerModule.Services
{
    class AnalyzerConfigurationService : IAnalyzerConfigurationService
    {
        private readonly Dictionary<string, AnalyzerID> _analyzerConfigurationDictionary;
 
        public AnalyzerConfigurationService()
        {
            _analyzerConfigurationDictionary = new Dictionary<string, AnalyzerID>();
            _analyzerConfigurationDictionary.Add(Properties.Settings.Default.A, AnalyzerID.A);
            _analyzerConfigurationDictionary.Add(Properties.Settings.Default.B, AnalyzerID.B);
            _analyzerConfigurationDictionary.Add(Properties.Settings.Default.C, AnalyzerID.C);
            _analyzerConfigurationDictionary.Add(Properties.Settings.Default.D, AnalyzerID.D);
            _analyzerConfigurationDictionary.Add(Properties.Settings.Default.E, AnalyzerID.E);
            _analyzerConfigurationDictionary.Add(Properties.Settings.Default.F, AnalyzerID.F);
            _analyzerConfigurationDictionary.Add(Properties.Settings.Default.G, AnalyzerID.G);
            _analyzerConfigurationDictionary.Add(Properties.Settings.Default.H, AnalyzerID.H);
        }

        public AnalyzerID GetAnalyzerID(string serialNumber)
        {
            return _analyzerConfigurationDictionary[serialNumber];
        }

        public int GetAnalyzerCount()
        {
            return _analyzerConfigurationDictionary.Count;
        }
    }
}
