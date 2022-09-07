using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;

namespace DigitusTestCase.WebAPP.Services.EmailService
{
    public class EmailService: IEmailService
    {

        public bool Send(string userEmail, string confirmationLink)
        {
            //https://ethereal.email/create
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("dario13@ethereal.email"));
            email.To.Add(MailboxAddress.Parse(userEmail));
            email.Subject = "Confirm your email";
            email.Body = new TextPart(TextFormat.Html) { Text =confirmationLink};

            // send email
            var smtp = new SmtpClient();
            smtp.Connect("smtp.ethereal.email", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("dario13@ethereal.email", "KpSkz3eQtth53SJnht");
            var isSend=smtp.Send(email);
            smtp.Disconnect(true);
            return true;
        }
    }
}
 