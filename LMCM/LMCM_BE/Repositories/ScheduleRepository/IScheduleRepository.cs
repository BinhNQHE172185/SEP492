using LMCM_BE.DTOs.CLODtos;
using LMCM_BE.DTOs.ScheduleDtos;

namespace LMCM_BE.Repositories.ScheduleRepository
{
    public interface IScheduleRepository
    {
        Task<bool> ImportSchedulesAsync(List<ScheduleInsertDto> schedules);
        Task<bool> DeleteSchedulesBySyllabusAsync(Guid syllabusId);
    }
}
