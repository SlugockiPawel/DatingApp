using CloudinaryDotNet.Actions;
using DatingApp.DTOs;
using DatingApp.Models;

namespace DatingApp.Services.Interfaces;

public interface IPhotoService
{
    Task<ImageUploadResult> AddPhotoAsync(IFormFile file);
    Task<DeletionResult> DeletePhotoAsync(string publicId);
    Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotosAsync();
    Task<Photo> GetPhotoByIdAsync(int id);
    void RemovePhoto(Photo photo);
}