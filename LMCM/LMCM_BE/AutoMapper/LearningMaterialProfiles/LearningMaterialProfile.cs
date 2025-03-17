using AutoMapper;
using LMCM_BE.DTOs.LearningMaterialDtos;
using LMCM_BE.Models;

namespace LMCM_BE.AutoMapper.LearningMaterialProfiles
{
    public class LearningMaterialProfile : Profile
    {
        public LearningMaterialProfile()
        {
            CreateMap<LearningMaterial, LearningMaterialInsertDto>();
            CreateMap<LearningMaterialImportDto, LearningMaterial>()
                .ForMember(dest => dest.MaterialId, opt => opt.Ignore())
                .ForMember(dest => dest.IsMainMaterial, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
            CreateMap<LearningMaterialInsertDto, LearningMaterial>()
                .ForMember(dest => dest.MaterialId, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
            CreateMap<LearningMaterialUpdateDto, LearningMaterial>()
                .ForMember(dest => dest.MaterialId, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.MaterialDetail, opt => opt.MapFrom(src => src.MaterialDetail));
            CreateMap<LearningMaterial, LearningMaterialListDto>();
            CreateMap<LearningMaterial, LearningMaterialViewDto>()
                .ForMember(dest => dest.MaterialDetail, opt => opt.MapFrom(src => src.MaterialDetail));
            CreateMap<LearningMaterialViewDto, LearningMaterial>()
                .ForMember(dest => dest.MaterialDetail, opt => opt.MapFrom(src => src.MaterialDetail));
            CreateMap<LearningMaterialUpdateDto, LearningMaterialInsertDto>();
        }
    }
}
