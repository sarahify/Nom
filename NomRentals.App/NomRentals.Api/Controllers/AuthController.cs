using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NomRentals.Api.Data;
using NomRentals.Api.Entities;
using NomRentals.Api.Models;
using NomRentals.Api.Services.AuthService;
using NomRentals.Api.Services.EmailService;
using System.Linq;
using System.Security.Cryptography;

namespace NomRentals.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly CustomerApiDbContext _dbContext;
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;

        public AuthController(CustomerApiDbContext dbContext, IAuthService authService, IEmailService emailService)
        {
            _dbContext= dbContext;
            _authService = authService;
            _emailService = emailService;


        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterRequest request)
        {
            if (_dbContext.Users.Any(u => u.Email == request.EmailAddress))
            {
                return BadRequest("User already exists.");
            }
            CreatePasswordHash(request.Password,
                out byte[] passwordHash,
                out byte[] passwordSalt);

            var user = new User
            {
                Email = request.EmailAddress,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                VerificationToken = CreateRandomToken()
            };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return Ok("User Successfully Created!");

        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac
                    .ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
            throw new NotImplementedException();
        }

        private string CreateRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
            throw new NotImplementedException();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginRequest request)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                return BadRequest("User not found.");

            }
            if (!VerifiedPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Password is incorrect!");
            }
            if (user.VerifiedAt == null)
            {
                return BadRequest("Not Verified!");
            }        
       
        }

        private bool VerifiedPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computeHash = hmac
                    .ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computeHash.SequenceEqual(passwordHash);
            }
            throw new NotImplementedException();
        }

        [HttpPost("verify")]
        public async Task<IActionResult> Verify(string token)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.VerificationToken == token);
            if (user == null)
            {
                return BadRequest("Invalid Token");
            }
            user.VerifiedAt = DateTime.Now;
            await _dbContext.SaveChangesAsync();
            return Ok("User Verified!");

        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(string email) 
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
            if(user == null) 
            {
                return BadRequest("User not found.");
            }
            user.PasswordResetToken = CreateRandomToken();
            user.ResetTokenExpires =DateTime.Now.AddDays(1);
            await _dbContext.SaveChangesAsync();

            return Ok("You may now reset your password.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request) 
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == request.Token);
            if(user == null || user.ResetTokenExpires<DateTime.Now) 
            {
                return BadRequest("Invalid Token.");
            }
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.PasswordResetToken = null;
            user.ResetTokenExpires = null;
            await _dbContext.SaveChangesAsync();
            return Ok("Password successfully reset.");
        }
    }


}


