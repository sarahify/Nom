using Microsoft.AspNetCore.Identity;
using NomRentals.Api.Entities;

namespace NomRentals.Api.Repository
{
    public class AccountRepository : IAccountRepository
    {
        public Task<string> LoginAsync(SignInModel signIn)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> SignUpAsync(SignUp signUp, string role)
        {
            throw new NotImplementedException();
        }
    }
}
