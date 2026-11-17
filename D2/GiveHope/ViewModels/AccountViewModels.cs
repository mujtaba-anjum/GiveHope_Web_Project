using System.ComponentModel.DataAnnotations;

namespace GiveHope.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;
}

public class RegisterViewModel
{
    [Required]
    [StringLength(100)]
    [Display(Name = "Name")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Display(Name = "I am a")]
    public string Role { get; set; } = "Donor";

    [StringLength(100)]
    [Display(Name = "City")]
    public string? City { get; set; }
}
