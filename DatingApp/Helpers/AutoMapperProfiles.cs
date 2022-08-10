using AutoMapper;
using DatingApp.DTOs;
using DatingApp.Extensions;
using DatingApp.Models;

namespace DatingApp.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<AppUser, MemberDto>()
            .ForMember(
                member => member.PhotoUrl,
                options => options.MapFrom(u => u.Photos.FirstOrDefault(p => p.IsMain).Url)
            )
            .ForMember(
                member => member.Age,
                options => options.MapFrom(u => u.DateOfBirth.CalculateAge())
            );
        CreateMap<Photo, PhotoDto>();
        CreateMap<MemberUpdateDto, AppUser>();
        CreateMap<RegisterDto, AppUser>();
        CreateMap<Message, MessageDto>()
            .ForMember(
                dest => dest.SenderPhotoUrl,
                opt => opt.MapFrom(m => m.Sender.Photos.FirstOrDefault(p => p.IsMain).Url)
            )
            .ForMember(
                dest => dest.RecipientPhotoUrl,
                opt => opt.MapFrom(m => m.Recipient.Photos.FirstOrDefault(p => p.IsMain).Url)
            );
        CreateMap<DateTime, DateTime>()
            .ConstructUsing(d => DateTime.SpecifyKind(d, DateTimeKind.Utc));
    }
}