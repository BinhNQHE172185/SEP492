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
        }
    }
}
