﻿using Microsoft.AspNetCore.Identity;

namespace DatingApp.Models;

public class AppUser : IdentityUser<Guid>
{
    public DateTime DateOfBirth { get; set; }
    public string KnownAs { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime LastActive { get; set; } = DateTime.UtcNow;
    public string Gender { get; set; }
    public string Introduction { get; set; }
    public string LookingFor { get; set; }
    public string Interests { get; set; }
    public string City { get; set; }
    public string Country { get; set; }

    // Navigation property 1 User => many Photos
    public virtual ICollection<Photo> Photos { get; set; }

    // users that likes currently logged in user
    public virtual ICollection<AppUserLike> LikedByUsers { get; set; }

    // users that are liked by currently logged in user
    public virtual ICollection<AppUserLike> LikedUsers { get; set; }

    public ICollection<Message> MessagesSent { get; set; }
    public ICollection<Message> MessagesRecieved { get; set; }
    public ICollection<AppUserRole> UserRoles { get; set; }
}