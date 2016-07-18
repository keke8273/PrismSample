using System.CodeDom;

namespace QBR.Infrastructure.Constants
{
    public static class ModuleInitializationPriority
    {
        //The smaller the priority value is, the higher the priority is.
        public const int ApplicationSettingsManagerModuleInitPriority = -2;
        public const int SplashModuleInitPriority = -1;
        public const int SoftwareUpgradeModuleInitPriority = 1;
        public const int TestManagerModuleInitPriority = 2;
        public const int UserDataModuleInitPriority = 3;
        public const int SoundModulePriority = 4;
        public const int AnalyzerManagerModuleInitPriority = 5;
        public const int RecordManagerModuleInitPriority = 6;
    }
}
