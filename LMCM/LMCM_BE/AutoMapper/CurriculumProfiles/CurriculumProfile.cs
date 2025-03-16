using AutoMapper;
using LMCM_BE.DTOs.CurriculumDtos;
using LMCM_BE.DTOs.CurriculumsSubjectDtos;
using LMCM_BE.DTOs.PloDtos;
using LMCM_BE.DTOs.PloSubjectDtos;
using LMCM_BE.Models;
using System.Linq;

namespace LMCM_BE.AutoMapper.CurriculumProfiles
{
    public class CurriculumProfile : Profile
    {
        public CurriculumProfile()
        {
            CreateMap<Curriculum, CurriculumDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.CurriculumName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.CurriculumDescription))
                .ForMember(dest => dest.TotalCredit, opt => opt.MapFrom(src => src.CurriculumsSubjects.Sum(cs => cs.Credit ?? 0)))
                .ReverseMap();
            CreateMap<Curriculum, CurriculumDetailDto>()
           .ForMember(dest => dest.Subjects, opt => opt.MapFrom(src => src.CurriculumsSubjects));

            CreateMap<CurriculumsSubject, TempCurriculumsSubject>()
              .ForMember(dest => dest.SubjectId, opt => opt.MapFrom(src => src.SubjectId))
              .ForMember(dest => dest.SubjectCode, opt => opt.MapFrom(src => src.Subject.SubjectCode))
              .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject.SubjectName))
              .ForMember(dest => dest.TermNo, opt => opt.MapFrom(src => src.TermNo))
              .ForMember(dest => dest.Credit, opt => opt.MapFrom(src => src.Credit))
              .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options));
            CreateMap<Plo, PloDetailDto>()
             .ForMember(dest => dest.Subjects, opt => opt.MapFrom(src => src.PloSubjects));

            CreateMap<PloSubject, TempPloSubject>()
                .ForMember(dest => dest.SubjectId, opt => opt.MapFrom(src => src.Subject.SubjectId))
                .ForMember(dest => dest.SubjectCode, opt => opt.MapFrom(src => src.Subject.SubjectCode))
                .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject.SubjectName));
        }
    }
}
