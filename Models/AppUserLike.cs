﻿namespace DatingApp.Models;

public sealed class AppUserLike
{
    public AppUser SourceUser { get; set; }
    public Guid SourceUserId { get; set; }

    public AppUser LikedUser { get; set; }
    public Guid LikedUserId { get; set; }
}