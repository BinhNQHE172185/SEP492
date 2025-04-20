using AutoMapper;
using LMCM_BE.DTOs.LearningMaterialChangesHistoryDtos;
using LMCM_BE.DTOs.LearningMaterialDtos;
using LMCM_BE.Models;

namespace LMCM_BE.AutoMapper.LearningMaterialChangesHistoryProfile
{
    public class LearningMaterialChangesHistoryProfile : Profile
    {
        public LearningMaterialChangesHistoryProfile()
        {
            CreateMap<LearningMaterialChangesHistory, ChangesHistoryListDto>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.ContractTitle, opt => opt.MapFrom(src => src.Contract != null ? src.Contract.Title : null));
            CreateMap<CreateLearningMaterialChangesHistoryDto, LearningMaterialChangesHistory>();
            CreateMap<UpdateLearningMaterialChangesHistoryDto, LearningMaterialChangesHistory>();
            CreateMap<LearningMaterialChangesHistory, ChangesHistoryOfSubjectDto>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.ContractTitle, opt => opt.MapFrom(src => src.Contract != null ? src.Contract.Title : null))
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Syllabus.CourseName));
            CreateMap<LearningMaterial, LearningMaterialViewDto>();
            CreateMap<LearningMaterialChangesHistory, ChangesHistoryDetailDto>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.ContractTitle, opt => opt.MapFrom(src => src.Contract != null ? src.Contract.Title : null))
                .ReverseMap();
        }
    }
}
