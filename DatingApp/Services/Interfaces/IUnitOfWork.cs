namespace DatingApp.Services.Interfaces;

public interface IUnitOfWork
{
    IUserService UserService { get; }
    IMessageService MessageService { get; }
    ILikeService LikeService { get; }
    Task<bool> Complete();
    bool HasChanges();
}