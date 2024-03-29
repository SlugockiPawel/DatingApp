﻿namespace DatingApp.DTOs;

public sealed class LikeDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public int Age { get; set; }
    public string KnownAs { get; set; }
    public string PhotoUrl { get; set; }
    public string City { get; set; }
    public bool LikedByCurrentUser { get; set; }
}