using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NomRentals.Api.Entities;

namespace NomRentals.Api.Repository
{
    public interface IAccountRepository
    {
        Task<IdentityResult> SignUpAsync(SignUp signUp, string role);
        Task<string> LoginAsync(SignInModel signIn);
        
    }
}
