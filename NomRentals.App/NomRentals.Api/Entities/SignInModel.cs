using System.ComponentModel.DataAnnotations;

namespace NomRentals.Api.Entities
{
    public class SignInModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
