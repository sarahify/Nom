using NomRentals.Api.Models;

namespace NomRentals.Api.Services.EmailService
{
    public interface IEmailService
    {
        void SendEmail(EmailDto request);
    }
}
