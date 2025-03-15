using AutoMapper;
using LMCM_BE.DTOs.ContractDtos;
using LMCM_BE.Models;

namespace LMCM_BE.AutoMapper.ContractProfiles
{
    public class ContractProfile : Profile
    {
        public ContractProfile()
        {
            CreateMap<ContractInsertDto, Contract>()
                .ForMember(dest => dest.ContractId, opt => opt.Ignore())
                .ForMember(dest => dest.Url, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore()) 
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) 
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore()); ;
        }
    }
}
