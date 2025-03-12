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
            CreateMap<Syllabus, SyllabusInsertDto>()
                .ReverseMap();
            CreateMap<Syllabus, SyllabusChangesHistoryListDto>();
        }
    }
}
