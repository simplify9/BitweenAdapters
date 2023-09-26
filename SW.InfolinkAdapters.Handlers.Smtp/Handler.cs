using Newtonsoft.Json;
using SW.PrimitiveTypes;
using SW.Serverless.Sdk;

namespace SW.InfolinkAdapters.Handlers.Smtp;

public class Handler : IInfolinkHandler
{
    public Handler()
    {
        Runner.Expect(CommonProperties.Port);
        Runner.Expect(CommonProperties.Host);
        Runner.Expect(CommonProperties.From);
        Runner.Expect(CommonProperties.Password);
        Runner.Expect(CommonProperties.To, null);
        Runner.Expect("Cc", null);
        Runner.Expect("Bcc", null);
    }

    private static List<string> GetMailList(List<string>? emailModelBcc, string startupValueKey)
    {
        var emails = new List<string>();

        if (emailModelBcc is not null)
        {
            emails.AddRange(emailModelBcc);
        }

        var value = Runner.StartupValueOf(startupValueKey);
        if (!string.IsNullOrEmpty(value))
        {
            emails.AddRange(value.Split(",").Select(i => i.Trim()));
        }

        return emails;
    }

    public Task<XchangeFile> Handle(XchangeFile xchangeFile)
    {
        var emailModel = JsonConvert.DeserializeObject<InputModel>(xchangeFile.Data);

        var startupEmail = Runner.StartupValueOf(CommonProperties.Host);
        var emailTo = string.IsNullOrEmpty(startupEmail) ? emailModel.To : startupEmail;

        Mailer.SendEmail(
            Runner.StartupValueOf(CommonProperties.Host),
            Convert.ToInt32(Runner.StartupValueOf(CommonProperties.Port)),
            Runner.StartupValueOf(CommonProperties.From),
            Runner.StartupValueOf(CommonProperties.Password),
            emailTo,
            emailModel.OtherTo,
            GetMailList(emailModel.Cc, "Cc"),
            GetMailList(emailModel.Bcc, "Bcc"),
            emailModel.Subject,
            emailModel.Body,
            emailModel.IsHtml,
            emailModel.AttachmentName,
            emailModel.AttachmentBody,
            false
        );
        return Task.FromResult(new XchangeFile(string.Empty));
    }
}