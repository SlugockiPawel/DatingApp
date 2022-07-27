using Microsoft.AspNetCore.Identity;

namespace DatingApp.Models;

public class AppRole : IdentityRole<Guid>
{
    public ICollection<AppUserRole> UserRoles { get; set; }
}