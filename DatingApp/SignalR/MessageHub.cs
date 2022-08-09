using AutoMapper;
using DatingApp.DTOs;
using DatingApp.Extensions;
using DatingApp.Models;
using DatingApp.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace DatingApp.SignalR;

// TODO make message hub authorized?
public class MessageHub : Hub
{
    private readonly IMapper _mapper;
    private readonly IMessageService _messageService;
    private readonly IUserService _userService;

    public MessageHub(IMessageService messageService, IUserService userService, IMapper mapper)
    {
        _messageService = messageService;
        _userService = userService;
        _mapper = mapper;
    }

    public override async Task OnConnectedAsync()
    {
        var loggedUser = Context.User.GetUserName();
        var httpContext = Context.GetHttpContext();
        var otherUser = httpContext.Request.Query["user"].ToString();
        var groupName = GetGroupName(loggedUser, otherUser);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await AddToGroup(groupName);

        var messages = await _messageService.GetMessageThread(loggedUser, otherUser);

        await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        await RemoveFromMessageGroup();
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(CreateMessageDto createMessageDto)
    {
        var username = Context.User.GetUserName();

        if (username == createMessageDto.RecipientName.ToLower())
            throw new HubException("You cannot send messages to yourself");

        var sender = await _userService.GetUserByNameAsync(username);
        var recipient = await _userService.GetUserByNameAsync(createMessageDto.RecipientName);

        if (recipient is null)
        {
            throw new HubException("Not found user");
        }

        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderName = sender.UserName,
            RecipientName = recipient.UserName,
            Content = createMessageDto.Content
        };

        var groupName = GetGroupName(sender.UserName, recipient.UserName);

        var group = await _messageService.GetMessageGroupAsync(groupName);

        if (group.Connections.Any(c => c.Username == recipient.UserName))
        {
            message.DateRead = DateTime.UtcNow;
        }
        
        await _messageService.AddMessageAsync(message);

        if (await _messageService.SaveAllAsync())
        {
            await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
        }
    }

    private async Task<bool> AddToGroup(string groupName)
    {
        var group = await _messageService.GetMessageGroupAsync(groupName);
        var connection = new Connection(Context.ConnectionId, Context.User.GetUserName());

        if (group is null)
        {
            group = new Group(groupName);
            await _messageService.AddGroupAsync(group);
        }

        group.Connections.Add(connection);

        return await _messageService.SaveAllAsync();
    }

    private async Task RemoveFromMessageGroup()
    {
        var connection = await _messageService.GetConnectionAsync(Context.ConnectionId);
        _messageService.RemoveConnection(connection);
        await _messageService.SaveAllAsync();
    }

    private string GetGroupName(string callerUser, string otherUser)
    {
        var stringCompare = string.CompareOrdinal(callerUser, otherUser) < 0;
        return stringCompare ? $"{callerUser}-{otherUser}" : $"{otherUser}-{callerUser}";
    }
}