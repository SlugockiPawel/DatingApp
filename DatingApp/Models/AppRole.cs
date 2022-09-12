using Microsoft.AspNetCore.Identity;

namespace DatingApp.Models;

public sealed class AppRole : IdentityRole<Guid>
{
    public ICollection<AppUserRole> UserRoles { get; set; }
}