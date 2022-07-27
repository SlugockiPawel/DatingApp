using Microsoft.AspNetCore.Identity;

namespace DatingApp.Models;

/// <summary>
///     represents a joint table for many to many relationship between AppUser and UserRoles
/// </summary>
public class AppUserRole : IdentityUserRole<Guid>
{
    public AppUser User { get; set; }
    public AppRole Role { get; set; }
}