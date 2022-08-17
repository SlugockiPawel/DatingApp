using System.ComponentModel.DataAnnotations.Schema;

namespace DatingApp.Models;

[Table("Photos")]
public class Photo
{
    public int Id { get; set; }
    public string Url { get; set; }
    public bool IsMain { get; set; }
    public string PublicId { get; set; }
    public bool IsApproved { get; set; }

    // Navigation property 1 User => many photos
    public virtual AppUser AppUser { get; set; }
    public Guid AppUserId { get; set; }
}