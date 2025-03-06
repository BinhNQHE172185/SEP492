using LMCM_BE.DTOs.ScheduleDtos;

namespace LMCM_BE.Services.ScheduleService
{
    public interface IScheduleService
    {
        Task<bool> ImportSchedulesAsync(List<ScheduleInsertDto> schedules);
        Task<bool> DeleteSchedulesBySyllabusAsync(Guid syllabusId);
    }
}
