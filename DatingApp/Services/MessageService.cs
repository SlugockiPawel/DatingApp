using AutoMapper;
using AutoMapper.QueryableExtensions;
using DatingApp.Data;
using DatingApp.DTOs;
using DatingApp.Helpers;
using DatingApp.Models;
using DatingApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Services;

public class MessageService : IMessageService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public MessageService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task AddMessageAsync(Message message)
    {
        await _context.Messages.AddAsync(message);
    }

    public void DeleteMessage(Message message)
    {
        _context.Remove(message);
    }

    public async Task<Message> GetMessageAsync(int id)
    {
        return await _context.Messages.FindAsync(id);
    }

    public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
    {
        var query = _context.Messages.OrderByDescending(m => m.DateSent).AsQueryable();

        query = messageParams.Container switch
        {
            "Inbox"
                => query.Where(
                    m => m.RecipientName == messageParams.Username && m.RecipientDeleted == false
                ),
            "Outbox"
                => query.Where(
                    m => m.SenderName == messageParams.Username && m.SenderDeleted == false
                ),
            _
                => query.Where(
                    m =>
                        m.RecipientName == messageParams.Username
                        && m.DateRead == null
                        && m.RecipientDeleted == false
                )
        };

        var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);

        return await PagedList<MessageDto>.CreateAsync(
            messages,
            messageParams.PageNumber,
            messageParams.PageSize
        );
    }

    public async Task<IEnumerable<MessageDto>> GetMessageThread(
        string currentUsername,
        string recipientUsername
    )
    {
        var messages = await _context.Messages
            .Include(m => m.Sender)
            .ThenInclude(u => u.Photos)
            .Include(m => m.Recipient)
            .ThenInclude(u => u.Photos)
            .Where(
                m =>
                    (
                        m.RecipientName == currentUsername
                        && m.RecipientDeleted == false
                        && m.SenderName == recipientUsername
                    )
                    || (
                        m.RecipientName == recipientUsername
                        && m.SenderName == currentUsername
                        && m.SenderDeleted == false
                    )
            )
            .OrderBy(m => m.DateSent)
            .ToListAsync();

        var unreadMessages = messages
            .Where(m => m.DateRead is null && m.RecipientName == currentUsername)
            .ToList();

        foreach (var message in unreadMessages)
            message.DateRead = DateTime.Now;
        await _context.SaveChangesAsync();

        return _mapper.Map<IEnumerable<MessageDto>>(messages);
    }

    public async Task<bool> SaveAllAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}