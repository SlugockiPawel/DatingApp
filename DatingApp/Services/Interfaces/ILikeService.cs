using DatingApp.DTOs;
using DatingApp.Models;

namespace DatingApp.Services.Interfaces;

public interface ILikeService
{
    Task<AppUserLike> GetUserLike(Guid sourceUserId, Guid likedUserId);
    Task<AppUser> GetUserWithLikes(Guid userId);
    Task<IEnumerable<LikeDto>> GetUserLikes(string predicate, Guid userId);
}