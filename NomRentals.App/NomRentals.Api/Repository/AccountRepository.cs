using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NomRentals.Api.Entities;
using NomRentals.Api.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NomRentals.Api.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<UserProfile> _userManager;
        private readonly SignInManager<UserProfile> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;


        public AccountRepository(UserManager<UserProfile> userManager,SignInManager<UserProfile> signInManager,IConfiguration configuration, RoleManager<IdentityRole> roleManager) 
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration ?? throw new ArgumentNullException();
            _roleManager = roleManager;
        }


        public async Task<string> LoginAsync(SignInModel signIn)
        {
            var findemail = await _userManager.FindByEmailAsync(signIn.Email);
            if (findemail == null)
            {
                throw new Exception("Email does not exist");
            }

            var result = await _signInManager.PasswordSignInAsync(signIn.Email, signIn.Password, isPersistent: false, lockoutOnFailure: false);

            if (!await _userManager.CheckPasswordAsync(findemail, signIn.Password))
            {
                throw new Exception("Password is invalid");
            }
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, signIn.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var authSigninKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddDays(1),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigninKey, SecurityAlgorithms.HmacSha384Signature));
            if (result.Succeeded)
            {
                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            else
            {
                throw new Exception("Account not logged in");
            }
        }

        public async Task<IdentityResult> SignUpAsync(SignUp signUp, string role)
        {
            var existUser = await _userManager.FindByEmailAsync(signUp.Email);
            if (existUser != null)
            {
                throw new Exception("User already exists");
            }

            if (await _roleManager.RoleExistsAsync(role))
            {
                var user = new UserProfile()
                {
                    FirstName = signUp.FirstName,
                    LastName = signUp.LastName,
                    Email = signUp.Email,
                    
                };
                var result = await _userManager.CreateAsync(user, signUp.Password);
                if (!result.Succeeded)
                {

                    throw new Exception("User failed to create");
                }
                await _userManager.AddToRoleAsync(user, role);
                return result;
            }
            else
            {
                throw new Exception("This role does not exist");
            }
        }
    }
}
