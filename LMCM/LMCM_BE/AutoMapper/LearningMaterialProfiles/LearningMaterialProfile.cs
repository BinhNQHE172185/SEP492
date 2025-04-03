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
                .ForMember(dest => dest.IsImportedMaterial, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
            CreateMap<LearningMaterial, LearningMaterialListDto>()
                .ForMember(dest => dest.MaterialId, opt => opt.MapFrom(src => src.MaterialId))
                .ForMember(dest => dest.LearningType, opt => opt.MapFrom(src => src.LearningType))
                .ForMember(dest => dest.IsMainMaterial, opt => opt.MapFrom(src => src.IsMainMaterial))
                .ForMember(dest => dest.MaterialName, opt => opt.MapFrom(src => src.MaterialName))
                .ForMember(dest => dest.Isbn, opt => opt.MapFrom(src => src.Isbn))
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author))
                .ForMember(dest => dest.Publisher, opt => opt.MapFrom(src => src.Publisher))
                .ForMember(dest => dest.PublishedDate, opt => opt.MapFrom(src => src.PublishedDate))
                .ForMember(dest => dest.Edition, opt => opt.MapFrom(src => src.Edition))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Url))
                .ForMember(dest => dest.Purpose, opt => opt.MapFrom(src => src.Purpose))
                .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note)); CreateMap<LearningMaterial, LearningMaterialViewDto>();
            CreateMap<LearningMaterialViewDto, LearningMaterial>();
            CreateMap<LearningMaterialUpdateDto, LearningMaterialInsertDto>();
        }
    }
}
