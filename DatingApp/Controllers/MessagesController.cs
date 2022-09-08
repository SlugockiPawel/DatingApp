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
    private readonly IUnitOfWork _unitOfWork;

    public MessagesController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
    {
        var username = User.GetUserName();

        if (username == createMessageDto.RecipientName.ToLower())
            return BadRequest("You cannot send messages to yourself");

        var sender = await _unitOfWork.UserService.GetUserByNameAsync(username);
        var recipient = await _unitOfWork.UserService.GetUserByNameAsync(
            createMessageDto.RecipientName
        );

        if (recipient is null)
            return NotFound();

        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderName = sender.UserName,
            RecipientName = recipient.UserName,
            Content = createMessageDto.Content
        };

        await _unitOfWork.MessageService.AddMessageAsync(message);

        if (await _unitOfWork.Complete())
            return Ok(_mapper.Map<MessageDto>(message));

        return BadRequest("Failed to send message");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser(
        [FromQuery] MessageParams messageParams
    )
    {
        messageParams.UserName = User.GetUserName();

        var messages = await _unitOfWork.MessageService.GetMessagesForUser(messageParams);

        Response.AddPaginationHeader(
            messages.CurrentPage,
            messages.PageSize,
            messages.TotalCount,
            messages.TotalPages
        );

        return messages;
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage(int id)
    {
        var username = User.GetUserName();

        var message = await _unitOfWork.MessageService.GetMessageAsync(id);

        if (message.SenderName != username && message.RecipientName != username)
            return Unauthorized();

        if (message.SenderName == username)
            message.SenderDeleted = true;

        if (message.RecipientName == username)
            message.RecipientDeleted = true;

        if (message.SenderDeleted && message.RecipientDeleted)
            _unitOfWork.MessageService.DeleteMessage(message);

        if (await _unitOfWork.Complete())
        {
            return NoContent();
        }

        return BadRequest("Failed to delete a message");
    }
}