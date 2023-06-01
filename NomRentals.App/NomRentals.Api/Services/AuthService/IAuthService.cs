using NomRentals.Api.Models;

namespace NomRentals.Api.Services.AuthService
{
    public interface IAuthService
    {
        Task<ResponseDto<ResetPasswordRequest>> ResetPasswords(ResetPasswordRequest resetPassword);
        Task<ResponseDto<string>> ForgotPassword(string email);
    }
}
