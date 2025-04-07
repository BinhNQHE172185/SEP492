using LMCM_BE.Models;

namespace LMCM_BE.Repositories.ScheduleRepository
{
    public interface IScheduleRepository
    {
        Task<bool> ImportSchedulesAsync(List<Schedule> schedules, Guid syllabusId);
        Task<bool> DeleteSchedulesBySyllabusAsync(Guid syllabusId);
    }
}
