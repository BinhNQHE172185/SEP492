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
                .ForMember(dest => dest.PreviousVersionId, opt => opt.Ignore())// We'll set this manually
                .ForMember(dest => dest.Status, opt => opt.Ignore()) // We'll set this manually
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Manually set timestamps
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
            CreateMap<Syllabus, SyllabusChangesHistoryListDto>();
        }
    }
}
