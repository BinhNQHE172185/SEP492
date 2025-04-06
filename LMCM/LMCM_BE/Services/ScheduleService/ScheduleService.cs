using LMCM_BE.DTOs.ScheduleDtos;
using LMCM_BE.Repositories.ScheduleRepository;

namespace LMCM_BE.Services.ScheduleService
{
    public class ScheduleService : IScheduleService
    {
        private readonly IScheduleRepository _scheduleRepository;
        public ScheduleService(IScheduleRepository scheduleRepository)
        {
            _scheduleRepository = scheduleRepository;
        }
        public async Task<bool> DeleteSchedulesBySyllabusAsync(Guid syllabusId)
        {
            return await _scheduleRepository.DeleteSchedulesBySyllabusAsync(syllabusId);
        }

        public async Task<bool> ImportSchedulesAsync(List<ScheduleInsertDto> schedules, Guid syllabusId)
        {
            return await _scheduleRepository.ImportSchedulesAsync(schedules,syllabusId);
        }
    }
}
