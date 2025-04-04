using AutoMapper;
using LMCM_BE.DTOs.LearningMaterialDtos;
using LMCM_BE.Models;

namespace LMCM_BE.AutoMapper.LearningMaterialChangesHistoryProfile
{
    public class LearningMaterialChangesHistoryProfile : Profile
    {
        public LearningMaterialChangesHistoryProfile()
        {
            CreateMap<LearningMaterialChangesHistory, ChangesHistoryListDto>();
            CreateMap<CreateLearningMaterialChangesHistoryDto, LearningMaterialChangesHistory>();
            CreateMap<UpdateLearningMaterialChangesHistoryDto, LearningMaterialChangesHistory>();
            CreateMap<LearningMaterialChangesHistory, ChangesHistoryOfSubjectDto>()
               .ForMember(dest => dest.Contract, opt => opt.MapFrom(src => src.Contract))
               .ForMember(dest => dest.Syllabus, opt => opt.MapFrom(src => src.Syllabus))
               .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));
            CreateMap<LearningMaterial, LearningMaterialViewDto>();
        }
    }
}
