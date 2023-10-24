namespace SW.InfolinkAdapters.Handlers.Smtp;

public class InputModel
{
    public string To { set; get; }
    public List<string>? OtherTo { set; get; }
    public List<string>? Cc { set; get; }
    public List<string>? Bcc { set; get; }

    public string Subject { set; get; }
    public string Body { set; get; }
    public string? AttachmentName { set; get; }
    public string? AttachmentBody { set; get; }

    public bool IsHtml { set; get; } = false;
}