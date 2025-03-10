using AutoMapper;
using LMCM_BE.DTOs.LearningMaterialChangesHistoryProfilesDtos;
using LMCM_BE.Models;

namespace LMCM_BE.AutoMapper.LearningMaterialChangesHistoryProfiles
{
    public class LearningMaterialChangesHistoryProfile : Profile
    {
        public LearningMaterialChangesHistoryProfile()
        {
            CreateMap<LearningMaterialChangesHistory, ChangesHistoryListDto>();
        }
    }
}
