using AutoMapper;
using LMCM_BE.DTOs.LearningMaterialDtos;
using LMCM_BE.Models;

namespace LMCM_BE.AutoMapper.LearningMaterialProfiles
{
    public class LearningMaterialDetailsProfile : Profile
    {
        public LearningMaterialDetailsProfile()
        {
            CreateMap<LearningMaterialDetail, LearningMaterialDetailsInsertDto>();
            CreateMap<LearningMaterialDetailsInsertDto, LearningMaterialDetail>()
                .ForMember(dest => dest.MaterialDetailId, opt => opt.Ignore()) // Ignored since we generate a new ID
                .ForMember(dest => dest.Status, opt => opt.Ignore()) // Set manually
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Set manually
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
        }
    }
}
