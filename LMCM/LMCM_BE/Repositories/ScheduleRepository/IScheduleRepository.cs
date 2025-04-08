using LMCM_BE.Models;

namespace LMCM_BE.Repositories.ScheduleRepository
{
    public interface IScheduleRepository
    {
        Task<List<Schedule>> GetSchedulesBySyllabusAsync(Guid syllabusId);
        Task<bool> AddSchedulesAsync(List<Schedule> schedules);
        Task<bool> UpdateSchedulesAsync(List<Schedule> schedules);
    }
}
