using DatingApp.DTOs;
using DatingApp.Helpers;
using DatingApp.Models;

namespace DatingApp.Services.Interfaces;

public interface ILikeService
{
    Task<AppUserLike> GetUserLike(Guid sourceUserId, Guid likedUserId);
    Task<AppUser> GetUserWithLikes(Guid userId);
    Task<PagedList<LikeDto>> GetUserLikes(LikesParams likeParams);
}