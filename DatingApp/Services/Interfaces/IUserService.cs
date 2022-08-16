using DatingApp.DTOs;
using DatingApp.Helpers;
using DatingApp.Models;

namespace DatingApp.Services.Interfaces;

public interface IUserService
{
    void Update(AppUser user);
    Task<IEnumerable<AppUser>> GetAllUsersAsync();
    Task<AppUser> GetUserByIdAsync(Guid id);
    Task<AppUser> GetUserByNameAsync(string name);
    Task<UserDto> RegisterUserAsync(RegisterDto registerDto);
    Task<bool> UserExistAsync(string username);
    Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams);
    Task<MemberDto> GetMemberByNameAsync(string name);
    Task<string> GetUserGender(string username);
}