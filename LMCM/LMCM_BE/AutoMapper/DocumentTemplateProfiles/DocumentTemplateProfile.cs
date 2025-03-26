using AutoMapper;
using LMCM_BE.DTOs.DocumentTemplateDtos;
using LMCM_BE.Models;

namespace LMCM_BE.AutoMapper.TemplateProfiles
{
    public class DocumentTemplateProfile : Profile
    {
        public DocumentTemplateProfile()
        {
            CreateMap<DocumentTemplateInsertDto, DocumentTemplate>()
                .ForMember(dest => dest.TemplateId, opt => opt.Ignore())
                .ForMember(dest => dest.Url, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
            CreateMap<DocumentTemplate, DocumentTemplateListDto>()
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author));
            CreateMap<DocumentTemplate, DocumentTemplateDetailDto>()
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author));
            CreateMap<DocumentTemplateUpdateDto, DocumentTemplate>()
                .ForMember(dest => dest.TemplateId, opt => opt.Ignore())
                .ForMember(dest => dest.AuthorId, opt => opt.Ignore())
                .ForMember(dest => dest.Url, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
        }
    }
}
