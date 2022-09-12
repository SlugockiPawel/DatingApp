using AutoMapper;
using AutoMapper.QueryableExtensions;
using DatingApp.Data;
using DatingApp.DTOs;
using DatingApp.Helpers;
using DatingApp.Models;
using DatingApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Services;

public sealed class MessageService : IMessageService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public MessageService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public void RemoveConnection(Connection connection)
    {
        _context.Remove(connection);
    }

    public async Task<Connection> GetConnectionAsync(string connectionId)
    {
        return await _context.Connections.FindAsync(connectionId);
    }

    public async Task<Group> GetMessageGroupAsync(string groupName)
    {
        return await _context.Groups
            .Include(g => g.Connections)
            .FirstOrDefaultAsync(g => g.Name == groupName);
    }

    public async Task<Group> GetGroupForConnectionAsync(string connectionId)
    {
        return await _context.Groups
            .Include(g => g.Connections)
            .FirstOrDefaultAsync(g => g.Connections.Any(c => c.ConnectionId == connectionId));
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
        var query = _context.Messages
            .OrderByDescending(m => m.DateSent)
            .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
            .AsQueryable();

        var messages = messageParams.Container switch
        {
            "Inbox"
                => query.Where(
                    m => m.RecipientName == messageParams.UserName && m.RecipientDeleted == false
                ),
            "Outbox"
                => query.Where(
                    m => m.SenderName == messageParams.UserName && m.SenderDeleted == false
                ),
            _
                => query.Where(
                    m =>
                        m.RecipientName == messageParams.UserName
                        && m.DateRead == null
                        && m.RecipientDeleted == false
                )
        };

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
            .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        var unreadMessages = messages
            .Where(m => m.DateRead is null && m.RecipientName == currentUsername)
            .ToList();

        foreach (var message in unreadMessages)
        {
            message.DateRead = DateTime.UtcNow;
        }

        return messages;
    }

    public async Task AddGroupAsync(Group group)
    {
        await _context.Groups.AddAsync(group);
    }
}