using AutoMapper;
using DatingApp.DTOs;
using DatingApp.Extensions;
using DatingApp.Helpers;
using DatingApp.Models;
using DatingApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Controllers;

[Authorize]
public class MessagesController : BaseApiController
{
    private readonly IMapper _mapper;
    private readonly IMessageService _messageService;
    private readonly IUserService _userService;

    public MessagesController(
        IUserService userService,
        IMessageService messageService,
        IMapper mapper
    )
    {
        _userService = userService;
        _messageService = messageService;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
    {
        var username = User.GetUserName();

        if (username == createMessageDto.RecipientName.ToLower())
            return BadRequest("You cannot send messages to yourself");

        var sender = await _userService.GetUserByNameAsync(username);
        var recipient = await _userService.GetUserByNameAsync(createMessageDto.RecipientName);

        if (recipient is null)
            return NotFound();

        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderName = sender.Name,
            RecipientName = recipient.Name,
            Content = createMessageDto.Content
        };

        await _messageService.AddMessageAsync(message);

        if (await _messageService.SaveAllAsync())
            return Ok(_mapper.Map<MessageDto>(message));

        return BadRequest("Failed to send message");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser(
        [FromQuery] MessageParams messageParams
    )
    {
        messageParams.Username = User.GetUserName();

        var messages = await _messageService.GetMessagesForUser(messageParams);

        Response.AddPaginationHeader(
            messages.CurrentPage,
            messages.PageSize,
            messages.TotalCount,
            messages.TotalPages
        );

        return messages;
    }
}