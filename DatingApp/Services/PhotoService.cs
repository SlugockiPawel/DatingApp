using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.Helpers;
using DatingApp.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace DatingApp.Services;

public class PhotoService : IPhotoService
{
    private readonly Cloudinary _cloudinary;

    public PhotoService(Cloudinary cloudinary, IOptions<CloudinarySettings> cloudinaryConfig)
    {
        var cloudinaryAccount = new Account()
        {
            Cloud = cloudinaryConfig.Value.CloudName,
            ApiKey = cloudinaryConfig.Value.ApiKey,
            ApiSecret = cloudinaryConfig.Value.ApiSecret,
        };

        _cloudinary = new Cloudinary(cloudinaryAccount);
    }

    public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
    {
        var uploadResult = new ImageUploadResult();

        if (file.Length > 0)
        {
            await using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, stream),
                Transformation = new Transformation().Height(500).Width(500)
                    .Crop("fill").Gravity("face"),
            };

            uploadResult = await _cloudinary.UploadAsync(uploadParams);
        }

        return uploadResult;
    }

    public async Task<DeletionResult> DeletePhotoAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);
        return await _cloudinary.DestroyAsync(deleteParams);
    }
}