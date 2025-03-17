using AutoMapper;
using LMCM_BE.DTOs.BudgetProposalDtos;
using LMCM_BE.Models;

namespace LMCM_BE.AutoMapper.BudgetProposalProfile
{
    public class BudgetPropasalProfile : Profile
    {
        public BudgetPropasalProfile()
        {
            CreateMap<BudgetProposalInsertDto, BudgetProposal>()
                .ForMember(dest => dest.ProposalId, opt => opt.Ignore())
                .ForMember(dest => dest.Url, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
            CreateMap<BudgetProposal, BudgetProposalListDto>()
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author));
        }
    }
}
