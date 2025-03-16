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
        }
    }
}
