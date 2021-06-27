using System;

namespace SW.Infolink.Adapters.Handlers.Notifier.MsTeams
{
    public class NotificationModel
    {
        public string Id { get; set; }
        public bool Success { get; set; }
        public string Exception { get; set; }
        public DateTime FinishedOn { get; set; }
        public bool OutputBad { get; set; }
        public bool ResponseBad { get; set; }
    }
}