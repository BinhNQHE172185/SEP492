using AutoMapper;
using LMCM_BE.DTOs.ContractorDtos;
using LMCM_BE.Models;

namespace LMCM_BE.AutoMapper.ContractorProfile
{
    public class ContractorProfile : Profile
    {
        public ContractorProfile()
        {
            CreateMap<Contractor, ContractorListDto>();
            CreateMap<ContractorCreateDto, Contractor>();
            CreateMap<ContractorUpdateDto, Contractor>()
                .ForMember(dest => dest.Status, opt => opt.Ignore());
            CreateMap<ContractorDetailDto, Contractor>();
            CreateMap<Contractor, ContractorDetailDto>();
        }
    }
}
