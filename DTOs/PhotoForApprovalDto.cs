namespace DatingApp.DTOs;

public sealed class PhotoForApprovalDto
{
    public int Id { get; set; }
    public bool IsApproved { get; set; }
    public string Url { get; set; }
    public string UserName { get; set; }
}