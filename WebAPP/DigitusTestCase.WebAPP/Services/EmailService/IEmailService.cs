namespace DigitusTestCase.WebAPP.Services.EmailService
{
    public interface IEmailService
    {
        bool Send(string userEmail, string confirmationLink);
    }
}
