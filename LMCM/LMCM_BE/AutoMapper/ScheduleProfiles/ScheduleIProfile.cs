using AutoMapper;
using LMCM_BE.DTOs.CurriculumDtos;
using LMCM_BE.DTOs.ScheduleDtos;
using LMCM_BE.Models;

namespace LMCM_BE.AutoMapper.ScheduleProfiles
{
    public class ScheduleIProfile:Profile
    {
        public ScheduleIProfile()
        {
            CreateMap<Schedule, ScheduleInsertDto>();
            CreateMap<ScheduleInsertDto, Schedule>()
                .ForMember(dest => dest.ScheduleId, opt => opt.Ignore()) // Ignored since we generate a new ID
                .ForMember(dest => dest.Status, opt => opt.Ignore()) // We'll set this manually
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Manually set timestamps
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
        }
    }
}
