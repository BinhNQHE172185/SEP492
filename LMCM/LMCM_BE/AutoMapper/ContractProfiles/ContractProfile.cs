using AutoMapper;
using LMCM_BE.DTOs.BudgetProposalDtos;
using LMCM_BE.DTOs.ContractDtos;
using LMCM_BE.DTOs.ContractorDtos;
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
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
             CreateMap<Contract, ContractListDto>()
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author))
                .ForMember(dest => dest.Proposal, opt => opt.MapFrom(src => src.Proposal))
                .ForMember(dest => dest.Contractor, opt => opt.MapFrom(src => src.Contractor));
            CreateMap<Contract, ContractDetailDto>()
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author))
                .ForMember(dest => dest.Proposal, opt => opt.MapFrom(src => src.Proposal))
                .ForMember(dest => dest.Contractor, opt => opt.MapFrom(src => src.Contractor))
                .ForMember(dest => dest.LearningMaterialChangesHistories, opt => opt.MapFrom(src => src.LearningMaterialChangesHistories));
            CreateMap<ContractUpdateDto, Contract>()
                .ForMember(dest => dest.ContractId, opt => opt.Ignore())
                .ForMember(dest => dest.Url, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
        }
    }
}
