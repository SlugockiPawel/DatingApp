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
    private readonly IHubContext<PresenceHub> _presenceHubContext;
    private readonly PresenceTracker _presenceTracker;
    private readonly IUserService _userService;

    public MessageHub(
        IMessageService messageService,
        IUserService userService,
        IMapper mapper,
        IHubContext<PresenceHub> presenceHubContext,
        PresenceTracker presenceTracker
    )
    {
        _messageService = messageService;
        _userService = userService;
        _mapper = mapper;
        _presenceHubContext = presenceHubContext;
        _presenceTracker = presenceTracker;
    }

    public override async Task OnConnectedAsync()
    {
        var loggedUser = Context.User.GetUserName();
        var httpContext = Context.GetHttpContext();
        var otherUser = httpContext.Request.Query["user"].ToString();
        var groupName = GetGroupName(loggedUser, otherUser);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        var group = await AddToGroup(groupName);
        await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

        var messages = await _messageService.GetMessageThread(loggedUser, otherUser);

        await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var group = await RemoveFromMessageGroup();
        await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);

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
        else
        {
            var connections = await _presenceTracker.GetConnectionsForUser(recipient.UserName);
            if (connections is not null)
            {
                await _presenceHubContext.Clients
                    .Clients(connections)
                    .SendAsync(
                        "NewMessageReceived",
                        new { username = sender.UserName, knownAs = sender.KnownAs }
                    );
            }
        }

        await _messageService.AddMessageAsync(message);

        if (await _messageService.SaveAllAsync())
        {
            await Clients
                .Group(groupName)
                .SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
        }
    }

    private async Task<Group> AddToGroup(string groupName)
    {
        var group = await _messageService.GetMessageGroupAsync(groupName);
        var connection = new Connection(Context.ConnectionId, Context.User.GetUserName());

        if (group is null)
        {
            group = new Group(groupName);
            await _messageService.AddGroupAsync(group);
        }

        group.Connections.Add(connection);

        if (await _messageService.SaveAllAsync())
        {
            return group;
        }

        throw new HubException("Failed to join group");
    }

    private async Task<Group> RemoveFromMessageGroup()
    {
        var group = await _messageService.GetGroupForConnectionAsync(Context.ConnectionId);
        var connection = group.Connections.FirstOrDefault(
            c => c.ConnectionId == Context.ConnectionId
        );

        _messageService.RemoveConnection(connection);
        if (await _messageService.SaveAllAsync())
        {
            return group;
        }

        throw new HubException("Failed to remove from group");
    }

    private string GetGroupName(string callerUser, string otherUser)
    {
        var stringCompare = string.CompareOrdinal(callerUser, otherUser) < 0;
        return stringCompare ? $"{callerUser}-{otherUser}" : $"{otherUser}-{callerUser}";
    }
}