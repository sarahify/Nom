using Microsoft.AspNetCore.Identity;
using NomRentals.Api.Data;
using NomRentals.Api.Models;
using NomRentals.Api.Services.EmailService;

namespace NomRentals.Api.Services.AuthService
{
    public class AuthService: IAuthService
    {
        private readonly IEmailService _emailService;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly CustomerApiDbContext _dbContext;

        public AuthService(IEmailService emailService, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, CustomerApiDbContext dbContext)
        {
            _emailService = emailService;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _userManager = userManager;
            _dbContext = dbContext;
        }


        public async Task<ResponseDto<ResetPasswordRequest>> ResetPasswords(ResetPasswordRequest resetPassword)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(resetPassword.Token);
                if (user == null)
                {
                    var response = new ResponseDto<ResetPasswordRequest>
                    {
                        StatusCode = 404,
                        DisplayMessage = "User not found",
                        Result = null,
                        ErrorMessages = new List<string> { "The user with the specified email address was not found" }
                    };
                    return response;
                }

                var resetResult = await _userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.Password);

                if (resetResult.Succeeded)
                {
                    var response = new ResponseDto<ResetPasswordRequest>
                    {
                        StatusCode = 200,
                        DisplayMessage = "Password reset successful",
                        Result = resetPassword,
                        ErrorMessages = null
                    };
                    return response;
                }
                else
                {
                    var response = new ResponseDto<ResetPasswordRequest>
                    {
                        StatusCode = 500,
                        DisplayMessage = "Password reset failed",
                        Result = null,
                        ErrorMessages = new List<string> { "An error occurred while resetting the password" }
                    };
                    return response;
                }

            }
            catch (Exception ex)
            {
                var response = new ResponseDto<ResetPasswordRequest>
                {
                    StatusCode = 500,
                    DisplayMessage = "Internal Server Error",
                    Result = null,
                    ErrorMessages = new List<string> { $"{ex.Message}", "An error occurred while resetting the password" }
                };
                return response;
            }
        }
        public async Task<ResponseDto<string>> ForgotPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var tokens = await _userManager.GeneratePasswordResetTokenAsync(user);
                if (tokens != null)
                {
                    // Send email with the generated token
                    var message = new Message(new string[] { email }, "Reset Password Token", $"Your reset password token is: {tokens}");
                    _emailService.SendEmail(message);
                    return new ResponseDto<string>
                    {
                        StatusCode = StatusCodes.Status200OK,
                        DisplayMessage = $"Reset password token generated and sent to email: {email}",
                        Result = tokens
                    };
                }
                return new ResponseDto<string>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    DisplayMessage = "Token not generated",
                };
            }

            return new ResponseDto<string>
            {
                StatusCode = StatusCodes.Status404NotFound,
                DisplayMessage = $"Email not found: {email}",
                ErrorMessages = new List<string> { $"Email not found: {email}" }
            };
        }
    }

}
