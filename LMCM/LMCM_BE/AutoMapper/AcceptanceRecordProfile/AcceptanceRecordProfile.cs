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
            CreateMap<AcceptanceRecord, AcceptanceRecordListDto>()
                .ForMember(dest => dest.contractId, opt => opt.MapFrom(src => src.Contract.ContractId))
                .ForMember(dest => dest.contractTitle, opt => opt.MapFrom(src=>src.Contract.Title));
            CreateMap<AcceptanceRecordCreateDto, AcceptanceRecord>()
                .ForMember(dest => dest.Url, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
            CreateMap<AcceptanceRecordUpdateDto, AcceptanceRecord>()
                .ForMember(dest => dest.Url, opt => opt.Ignore()) 
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) 
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore()); 

            CreateMap<AcceptanceRecord, AcceptanceRecordDetailDto>()
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author))
                .ForMember(dest => dest.Contract, opt => opt.MapFrom(src => src.Contract));
        }
    }
}
