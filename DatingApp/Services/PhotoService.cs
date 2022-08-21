﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.Data;
using DatingApp.DTOs;
using DatingApp.Helpers;
using DatingApp.Models;
using DatingApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DatingApp.Services;

public class PhotoService : IPhotoService
{
    private readonly Cloudinary _cloudinary;
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public PhotoService(
        IOptions<CloudinarySettings> cloudinaryConfig,
        ApplicationDbContext context,
        IMapper mapper
    )
    {
        _context = context;
        _mapper = mapper;
        var cloudinaryAccount = new Account
        {
            Cloud = cloudinaryConfig.Value.CloudName,
            ApiKey = cloudinaryConfig.Value.ApiKey,
            ApiSecret = cloudinaryConfig.Value.ApiSecret
        };

        _cloudinary = new Cloudinary(cloudinaryAccount);
    }

    public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
    {
        var uploadResult = new ImageUploadResult();

        if (file.Length > 0)
        {
            await using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Transformation = new Transformation()
                    .Height(500)
                    .Width(500)
                    .Crop("fill")
                    .Gravity("face")
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

    public async Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotosAsync()
    {
        return await _context.Photos
            .IgnoreQueryFilters()
            .Where(p => !p.IsApproved)
            .ProjectTo<PhotoForApprovalDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<Photo> GetPhotoByIdAsync(int id)
    {
        return await _context.Photos.IgnoreQueryFilters().SingleOrDefaultAsync(p => p.Id == id);
    }

    public void RemovePhoto(Photo photo)
    {
        _context.Photos.Remove(photo);
    }
}