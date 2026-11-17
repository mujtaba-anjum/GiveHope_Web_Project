using Microsoft.AspNetCore.Identity;

namespace GiveHope.Models;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string? City { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
