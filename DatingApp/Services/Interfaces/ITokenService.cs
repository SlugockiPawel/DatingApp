using DatingApp.Models;

namespace DatingApp.Services.Interfaces;

public interface ITokenService
{
    Task<string> CreateTokenAsync(AppUser user);
}