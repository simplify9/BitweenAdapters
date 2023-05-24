using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace SW.InfolinkAdapters.Handlers.Smtp;

public static class Mailer
{
    public static void SendEmail(string smtpHost, int smtpPort, string fromEmail, string fromPassword,
        string toEmail,
        string subject, string body, bool isBodyHtml, string emailModelAttachmentName, string emailModelAttachmentBody)
    {
        using var mail = new MailMessage();
        mail.From = new MailAddress(fromEmail);
        mail.To.Add(toEmail);
        mail.Subject = subject;
        mail.Body = body;
        mail.IsBodyHtml = isBodyHtml;
        if (!string.IsNullOrEmpty(emailModelAttachmentName) && !string.IsNullOrEmpty(emailModelAttachmentBody))
        {
            var textBytes = Encoding.UTF8.GetBytes(emailModelAttachmentBody);
            var memoryStream = new MemoryStream(textBytes);
            var attachment = new Attachment(memoryStream, emailModelAttachmentName);
            mail.Attachments.Add(attachment);
        }

        using var smtp = new SmtpClient(smtpHost, smtpPort);
        smtp.Credentials = new NetworkCredential(fromEmail, fromPassword);
        smtp.EnableSsl = true;
        smtp.Send(mail);
    }
}