using System.Collections.Generic;

namespace SW.InfolinkAdapters.Handlers.Mailgun
{
    public class MailGunSendRequest
    {
        public string To { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public IEnumerable<string> AttachmentLocations { get; set; }
        public string Template { get; set; }
        public string Body { get; set; }
        public IDictionary<string, string> TemplateVariables { get; set; }
    }
}