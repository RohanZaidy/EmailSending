using System.ComponentModel.DataAnnotations;

namespace IdentityFramework.ViewModel
{
    public class RegisterVM
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;

        [Required]
        [Display(Name ="Confirm Password")]
        [Compare("Password",ErrorMessage ="Password and Confirm Password not match.")]
        public string ConfirmPassword { get; set; } = default!;
    }
}
