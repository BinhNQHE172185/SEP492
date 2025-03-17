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
           .ForMember(dest => dest.Semesters, opt => opt.MapFrom(src =>
               src.CurriculumsSubjects
                   .GroupBy(cs => cs.TermNo)
                   .Select(g => new SemesterDto
                   {
                       Name = $"Term {g.Key}", // Assign TermNo as Name
                       Number = g.Key,
                       SubjectCount = g.Count(),
                       semesterSubjects = g.Select(cs => new SemesterSubjectDto
                       {
                           SubjectId = cs.Subject.SubjectId,
                           SubjectCode = cs.Subject.SubjectCode,
                           SubjectName = cs.Subject.SubjectName,
                           SubjectNameEnglish = cs.Subject.SubjectNameEnglish,
                           Credit = cs.Credit,
                           Options = cs.Options,
                           Duration = cs.Subject.Duration,
                           Reality = cs.Subject.Reality
                       }).ToList()
                   }).ToList()));

            CreateMap<CurriculumsSubject, SemesterSubjectDto>()
                .ForMember(dest => dest.SubjectId, opt => opt.MapFrom(src => src.Subject.SubjectId))
                .ForMember(dest => dest.SubjectCode, opt => opt.MapFrom(src => src.Subject.SubjectCode))
                .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject.SubjectName))
                .ForMember(dest => dest.SubjectNameEnglish, opt => opt.MapFrom(src => src.Subject.SubjectNameEnglish))
                .ForMember(dest => dest.Credit, opt => opt.MapFrom(src => src.Credit))
                .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Subject.Duration))
                .ForMember(dest => dest.Reality, opt => opt.MapFrom(src => src.Subject.Reality));

            CreateMap<PloSubject, TempPloSubject>()
                .ForMember(dest => dest.SubjectId, opt => opt.MapFrom(src => src.Subject.SubjectId))
                .ForMember(dest => dest.SubjectCode, opt => opt.MapFrom(src => src.Subject.SubjectCode))
                .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject.SubjectName));
        }
    }
}
