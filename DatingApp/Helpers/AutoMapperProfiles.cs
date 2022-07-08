﻿using AutoMapper;
using DatingApp.DTOs;
using DatingApp.Extensions;
using DatingApp.Models;

namespace DatingApp.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, MemberDto>()
                .ForMember(member => member.PhotoUrl,
                    options => options.MapFrom(
                        u => u.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(member => member.Age, options => options.MapFrom(
                    u => u.DateOfBirth.CalculateAge()));
            CreateMap<Photo, PhotoDto>();
            CreateMap<MemberUpdateDto, AppUser>();      
        }
    }
}
