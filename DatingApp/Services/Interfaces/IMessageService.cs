using DatingApp.DTOs;
using DatingApp.Helpers;
using DatingApp.Models;

namespace DatingApp.Services.Interfaces;

public interface IMessageService
{
    void AddMessage(Message message);
    void DeleteMessage(Message message);
    Task<Message> GetMessage(int id);
    Task<PagedList<MessageDto>> GetMessagesForUser();
    Task<IEnumerable<MessageDto>> GetMessageThread(Guid currentUserId, Guid recipientId);
    Task<bool> SaveAllAsync();
}