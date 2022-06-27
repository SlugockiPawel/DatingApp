using DatingApp.Models;

namespace DatingApp.Services.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}
