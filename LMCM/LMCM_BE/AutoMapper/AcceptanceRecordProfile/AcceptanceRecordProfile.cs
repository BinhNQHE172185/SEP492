using AutoMapper;
using LMCM_BE.DTOs.AcceptanceRecordDtos;
using LMCM_BE.DTOs.ContractDtos;
using LMCM_BE.DTOs.ContractorDtos;
using LMCM_BE.DTOs.UserDtos;
using LMCM_BE.Models;

namespace LMCM_BE.AutoMapper.AcceptanceRecordProfile
{
    public class AcceptanceRecordProfile : Profile
    {
        public AcceptanceRecordProfile()
        {
            CreateMap<AcceptanceRecord, AcceptanceRecordListDto>();
            CreateMap<AcceptanceRecordCreateDto, AcceptanceRecord>();
            CreateMap<AcceptanceRecordUpdateDto, AcceptanceRecord>();

            CreateMap<AcceptanceRecord, AcceptanceRecordDetailDto>()
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author))
                .ForMember(dest => dest.Contract, opt => opt.MapFrom(src => src.Contract));

            CreateMap<User, ListUserResponseDto>();
            CreateMap<Contract, ContractListDto>();
        }
    }
}
