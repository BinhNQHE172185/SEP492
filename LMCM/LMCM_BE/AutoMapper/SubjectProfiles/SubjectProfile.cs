using AutoMapper;
using LMCM_BE.DTOs.SubjectDtos;
using LMCM_BE.Models;

namespace LMCM_BE.AutoMapper.SubjectProfile
{
    public class SubjectProfile : Profile
    {
        public SubjectProfile()
        {
            CreateMap<Subject, SubjectDto>();
            CreateMap<SubjectDto, Subject>()
                .ForMember(dest => dest.SubjectId, opt => opt.Ignore()) // Ignored since we generate a new ID
                .ForMember(dest => dest.Status, opt => opt.Ignore()) // We'll set this manually
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Manually set timestamps
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
        }
    }
}
