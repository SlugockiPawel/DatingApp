using AutoMapper;
using DatingApp.Extensions;
using DatingApp.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace DatingApp.SignalR;

public class MessageHub : Hub
{
    private readonly Mapper _mapper;
    private readonly IMessageService _messageService;

    public MessageHub(IMessageService messageService, Mapper mapper)
    {
        _messageService = messageService;
        _mapper = mapper;
    }

    public override async Task OnConnectedAsync()
    {
        var loggedUser = Context.User.GetUserName();
        var httpContext = Context.GetHttpContext();
        var otherUser = httpContext.Request.Query["user"].ToString();
        var groupName = GetGroupName(loggedUser, otherUser);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        var messages = await _messageService.GetMessageThread(loggedUser, otherUser);

        await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        await base.OnDisconnectedAsync(exception);
    }

    private string GetGroupName(string callerUser, string otherUser)
    {
        var stringCompare = string.CompareOrdinal(callerUser, otherUser) < 0;
        return stringCompare ? $"{callerUser}-{otherUser}" : $"{otherUser}-{callerUser}";
    }
}