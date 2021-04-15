using System.Collections.Generic;

namespace SW.InfolinkAdapters.Handlers.Sendgrid
{
    public class EmailRequest
    {
        public string To { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public IDictionary<string, string> AttachmentLocations { get; set; }
        public string Template { get; set; }
        public string Body { get; set; }
        public object TemplateVariables { get; set; }
    }
}