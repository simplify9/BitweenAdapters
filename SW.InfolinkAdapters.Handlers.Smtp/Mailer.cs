using System.Net;
using System.Net.Mail;

namespace SW.InfolinkAdapters.Handlers.Smtp;

public static class Mailer
{
    public static void SendEmail(string smtpHost, int smtpPort, string fromEmail, string fromPassword,
        string toEmail,
        string subject, string body, bool isBodyHtml)
    {
        using var mail = new MailMessage();
        mail.From = new MailAddress(fromEmail);
        mail.To.Add(toEmail);
        mail.Subject = subject;
        mail.Body = body;
        mail.IsBodyHtml = isBodyHtml;

        using var smtp = new SmtpClient(smtpHost, smtpPort);
        smtp.Credentials = new NetworkCredential(fromEmail, fromPassword);
        smtp.EnableSsl = true;
        smtp.Send(mail);
    }
}