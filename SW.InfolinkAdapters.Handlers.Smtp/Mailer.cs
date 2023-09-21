using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace SW.InfolinkAdapters.Handlers.Smtp;

public static class Mailer
{
    public static void SendEmail(string smtpHost, int smtpPort, string fromEmail, string fromPassword,
        string toEmail,
        List<string> otherTo, List<string> cc, List<string> bcc,
        string subject, string body, bool isBodyHtml, string emailModelAttachmentName, string emailModelAttachmentBody,
        bool enableSsl)
    {
        using var mail = new MailMessage();
        mail.From = new MailAddress(fromEmail);
        mail.To.Add(toEmail);

        foreach (var toMail in otherTo)
        {
            mail.To.Add(toMail);
        }

        foreach (var toMail in cc)
        {
            mail.CC.Add(toMail);
        }

        foreach (var toMail in bcc)
        {
            mail.Bcc.Add(toMail);
        }

        mail.Subject = subject;
        mail.Body = body;
        mail.IsBodyHtml = isBodyHtml;

        if (!string.IsNullOrEmpty(emailModelAttachmentName) && !string.IsNullOrEmpty(emailModelAttachmentBody))
        {
            var byteArray = Convert.FromBase64String(emailModelAttachmentBody);
            var memoryStream = new MemoryStream(byteArray);
            var attachment = new Attachment(memoryStream, emailModelAttachmentName);
            mail.Attachments.Add(attachment);
        }

        using var smtp = new SmtpClient(smtpHost, smtpPort);
        smtp.Credentials = new NetworkCredential(fromEmail, fromPassword);
        smtp.EnableSsl = enableSsl;
        smtp.Send(mail);
    }
}