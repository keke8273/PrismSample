using Microsoft.Practices.Prism.Interactivity.InteractionRequest;

namespace QBR.AnalyzerManagerModule.Models.Notifications
{
    public class TestResultNotification : Notification
    {
        public double ClotTime { get; set; }

        public double PartialFill { get; set; }

        public double DoulbeFill { get; set; }

        public double OBCValue { get; set; }

        public double MinimumCurrent { get; set; }
    }
}
