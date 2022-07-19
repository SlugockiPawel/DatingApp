namespace DatingApp.DTOs;

public class LikeDto
{
    public Guid UserId { get; set; }
    public string name { get; set; }
    public int Age { get; set; }
    public string KnownAs { get; set; }
    public string PhotoUrl { get; set; }
    public string City { get; set; }
}