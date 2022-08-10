namespace DatingApp.Models;

public class Message
{
    public int Id { get; set; }
    public Guid SenderId { get; set; }
    public string SenderName { get; set; }
    public AppUser Sender { get; set; }
    public Guid RecipientId { get; set; }
    public string RecipientName { get; set; }
    public AppUser Recipient { get; set; }
    public string Content { get; set; }
    public DateTime? DateRead { get; set; }
    public DateTime DateSent { get; set; } = DateTime.UtcNow;
    public bool SenderDeleted { get; set; }
    public bool RecipientDeleted { get; set; }
}