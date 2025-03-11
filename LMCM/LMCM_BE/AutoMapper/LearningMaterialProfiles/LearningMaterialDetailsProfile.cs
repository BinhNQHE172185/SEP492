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
        }
    }
}
