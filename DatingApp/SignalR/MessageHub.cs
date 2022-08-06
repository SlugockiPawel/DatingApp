using AutoMapper;
using DatingApp.DTOs;
using DatingApp.Extensions;
using DatingApp.Models;
using DatingApp.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace DatingApp.SignalR;

public class MessageHub : Hub
{
    private readonly Mapper _mapper;
    private readonly IMessageService _messageService;
    private readonly IUserService _userService;

    public MessageHub(IMessageService messageService, Mapper mapper, IUserService userService)
    {
        _messageService = messageService;
        _mapper = mapper;
        _userService = userService;
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

        await _messageService.AddMessageAsync(message);

        if (await _messageService.SaveAllAsync())
        {
            var group = GetGroupName(sender.UserName, recipient.UserName);
            await Clients.Group(group).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
        }
    }

    private string GetGroupName(string callerUser, string otherUser)
    {
        var stringCompare = string.CompareOrdinal(callerUser, otherUser) < 0;
        return stringCompare ? $"{callerUser}-{otherUser}" : $"{otherUser}-{callerUser}";
    }
}