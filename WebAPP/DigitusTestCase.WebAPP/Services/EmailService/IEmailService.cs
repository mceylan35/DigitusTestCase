namespace DigitusTestCase.WebAPP.Services.EmailService
{
    public interface IEmailService
    {
        void Send(string to, string subject, string html, string from = null);
    }
}
