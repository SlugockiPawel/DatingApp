using DatingApp.Models;

namespace DatingApp.Repos.Interfaces
{
    public interface IAppUserRepo
    {
        Task<IEnumerable<AppUser>> GetAllUsersAsync();
        Task<AppUser> GetUserByIdAsync(Guid id);
    }
}
