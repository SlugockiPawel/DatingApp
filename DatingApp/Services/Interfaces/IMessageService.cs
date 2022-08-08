using DatingApp.DTOs;
using DatingApp.Helpers;
using DatingApp.Models;

namespace DatingApp.Services.Interfaces;

public interface IMessageService
{
    void AddGroup(Group group);
    void RemoveConnection(Connection connection);
    Task<Connection> GetConnection(string connectionId);
    Task<Group> GetMessageGroup(string groupName);

    Task AddMessageAsync(Message message);
    void DeleteMessage(Message message);
    Task<Message> GetMessageAsync(int id);
    Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams);

    Task<IEnumerable<MessageDto>> GetMessageThread(
        string currentUsername,
        string recipientUsername
    );

    Task<bool> SaveAllAsync();
}