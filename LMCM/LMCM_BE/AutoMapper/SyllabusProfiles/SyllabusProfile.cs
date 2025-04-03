using AutoMapper;
using LMCM_BE.DTOs.SyllabusDtos;
using LMCM_BE.Models;

namespace LMCM_BE.AutoMapper.SyllabusProfiles
{
    public class SyllabusProfile : Profile
    {
        public SyllabusProfile()
        {
            CreateMap<Syllabus, SyllabusListViewDto>()
                .ForMember(dest => dest.IsApproved, opt => opt.MapFrom(src => src.ApprovedDate.HasValue))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.Status.ToLower() == "active"));
            CreateMap<Syllabus, SyllabusInsertDto>();
            CreateMap<SyllabusInsertDto, Syllabus>()
                .ForMember(dest => dest.SyllabusId, opt => opt.Ignore()) // Ignored since we generate a new ID
                .ForMember(dest => dest.Status, opt => opt.Ignore()) // We'll set this manually
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Manually set timestamps
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
            CreateMap<Syllabus, SyllabusDetailDto>()
                .ForMember(dest => dest.Schedules,
                opt => opt.MapFrom(src => src.Schedules.OrderBy(s => s.ScheduleNo)))
                .ForMember(dest => dest.Clos,
                opt => opt.MapFrom(src => src.Clos.OrderBy(s => s.CloName)))
                .ForMember(dest => dest.ConstructivistQuestions,
                opt => opt.MapFrom(src => src.ConstructivistQuestions.OrderBy(s => s.SessionNo)))
                .ForMember(dest => dest.GradingStructures,
                opt => opt.MapFrom(src => src.GradingStructures.OrderBy(s => s.StructureNo)));
        }
    }
}
