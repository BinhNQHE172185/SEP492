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
        }
    }
}
