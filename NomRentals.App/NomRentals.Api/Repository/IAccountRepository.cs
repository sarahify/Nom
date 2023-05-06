using Microsoft.AspNetCore.Identity;
using NomRentals.Api.Entities;

namespace NomRentals.Api.Repository
{
    public interface IAccountRepository
    {
        Task<IdentityResult> SignUpAsync(SignUp signUp, string role);
        Task<string> LoginAsync(SignInModel signIn);
    }
}
