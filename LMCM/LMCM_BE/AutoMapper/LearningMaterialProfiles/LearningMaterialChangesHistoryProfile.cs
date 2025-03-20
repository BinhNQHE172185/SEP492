using AutoMapper;
using LMCM_BE.DTOs.LearningMaterialDtos;
using LMCM_BE.Models;

namespace LMCM_BE.AutoMapper.LearningMaterialProfiles
{
    public class LearningMaterialChangesHistoryProfile : Profile
    {
        public LearningMaterialChangesHistoryProfile()
        {
            CreateMap<LearningMaterialChangesHistory, ChangesHistoryListDto>();
            CreateMap<CreateLearningMaterialChangesHistoryDto, LearningMaterialChangesHistory>();
            CreateMap<LearningMaterialChangesHistory, ChangesHistoryWithMaterialDto>()
           .ForMember(dest => dest.OldMaterial, opt => opt.MapFrom(src => src.OldMaterial))
           .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));
            CreateMap<LearningMaterial, LearningMaterialViewDto>();
        }
    }
}
