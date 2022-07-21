﻿namespace DatingApp.DTOs;

public class MessageDto
{
    public int Id { get; set; }
    public Guid SenderId { get; set; }
    public string SenderName { get; set; }
    public string SenderPhotoUrl { get; set; }
    public Guid RecipientId { get; set; }
    public string RecipientName { get; set; }
    public string RecipientPhotoUrl { get; set; }
    public string Content { get; set; }
    public DateTime? DateRead { get; set; }
    public DateTime DateSent { get; set; }
}