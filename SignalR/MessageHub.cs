using AutoMapper;
using DatingApp.DTOs;
using DatingApp.Extensions;
using DatingApp.Models;
using DatingApp.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace DatingApp.SignalR;

// TODO make message hub authorized?
public sealed class MessageHub : Hub
{
    private readonly IMapper _mapper;
    private readonly IHubContext<PresenceHub> _presenceHubContext;
    private readonly PresenceTracker _presenceTracker;
    private readonly IUnitOfWork _unitOfWork;

    public MessageHub(
        IMapper mapper,
        IHubContext<PresenceHub> presenceHubContext,
        PresenceTracker presenceTracker,
        IUnitOfWork unitOfWork
    )
    {
        _mapper = mapper;
        _presenceHubContext = presenceHubContext;
        _presenceTracker = presenceTracker;
        _unitOfWork = unitOfWork;
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

        var messages = await _unitOfWork.MessageService.GetMessageThread(loggedUser, otherUser);

        if (_unitOfWork.HasChanges())
        {
            await _unitOfWork.Complete();
        }

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

        var sender = await _unitOfWork.UserService.GetUserByNameAsync(username);
        var recipient = await _unitOfWork.UserService.GetUserByNameAsync(
            createMessageDto.RecipientName
        );

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

        var group = await _unitOfWork.MessageService.GetMessageGroupAsync(groupName);

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

        await _unitOfWork.MessageService.AddMessageAsync(message);

        if (await _unitOfWork.Complete())
        {
            await Clients
                .Group(groupName)
                .SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
        }
    }

    private async Task<Group> AddToGroup(string groupName)
    {
        var group = await _unitOfWork.MessageService.GetMessageGroupAsync(groupName);
        var connection = new Connection(Context.ConnectionId, Context.User.GetUserName());

        if (group is null)
        {
            group = new Group(groupName);
            await _unitOfWork.MessageService.AddGroupAsync(group);
        }

        group.Connections.Add(connection);

        if (await _unitOfWork.Complete())
        {
            return group;
        }

        throw new HubException("Failed to join group");
    }

    private async Task<Group> RemoveFromMessageGroup()
    {
        var group = await _unitOfWork.MessageService.GetGroupForConnectionAsync(
            Context.ConnectionId
        );
        var connection = group.Connections.FirstOrDefault(
            c => c.ConnectionId == Context.ConnectionId
        );

        _unitOfWork.MessageService.RemoveConnection(connection);
        if (await _unitOfWork.Complete())
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