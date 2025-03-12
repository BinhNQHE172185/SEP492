using AutoMapper;
using LMCM_BE.DTOs.LearningMaterialDtos;
using LMCM_BE.Models;

namespace LMCM_BE.AutoMapper.LearningMaterialProfiles
{
    public class LearningMaterialProfile: Profile
    {
        public LearningMaterialProfile()
        {
            CreateMap<LearningMaterial, LearningMaterialInsertDto>();
            CreateMap<LearningMaterialInsertDto, LearningMaterial>()
                .ForMember(dest => dest.MaterialId, opt => opt.Ignore()) // Ignored since we generate a new ID
                .ForMember(dest => dest.IsMainMaterial, opt => opt.Ignore()) // Set manually
                .ForMember(dest => dest.Status, opt => opt.Ignore()) // Set manually
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Set manually
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
        }
    }
}
