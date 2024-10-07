using System.ComponentModel.DataAnnotations;

namespace IdentityFramework.ViewModel
{
    public class LoginVM
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;

        [Display(Name ="Remember me")]
        public bool RememberMe { get; set; }
    }
}
