using System;

namespace SW.InfolinkAdapters
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