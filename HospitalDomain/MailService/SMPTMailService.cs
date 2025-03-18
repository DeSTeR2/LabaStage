using HospitalDomain.Utils;
using MailKit.Net.Smtp;
using MimeKit;

namespace HospitalDomain.MailService
{
    public class SMPTMailService : IMailService
    {
        public void SentMail(Mail mail)
        {
            var email = new MimeMessage();

            email.From.Add(new MailboxAddress(AuthorizedData.SENDER_NAME, AuthorizedData.SENDER_EMAIL));
            email.To.Add(new MailboxAddress(mail.recieverName, mail.recieverEmail));

            email.Subject = mail.subject;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Plain)
            {
                Text = mail.message
            };
            using (var smtp = new SmtpClient())
            {
                smtp.Connect(AuthorizedData.SMTP_SERVER, AuthorizedData.PORT, false);
                smtp.Authenticate(AuthorizedData.LOGIN, AuthorizedData.PASSWORD);

                smtp.Send(email);
                smtp.Disconnect(true);
            }
        }
    }
}
