using AutoMapper;
using LMCM_BE.DTOs.UserDtos;
using LMCM_BE.Models;

namespace LMCM_BE.AutoMapper.UserProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserProfileResponseDto>();
            CreateMap<User, ListUserResponseDto>();
        }
    }
}
