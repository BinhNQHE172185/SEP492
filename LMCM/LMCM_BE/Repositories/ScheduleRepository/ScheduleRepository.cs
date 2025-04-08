using LMCM_BE.DbContext;
using LMCM_BE.Models;
using Microsoft.EntityFrameworkCore;

namespace LMCM_BE.Repositories.ScheduleRepository
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly LMCM_DBContext _dbContext;
        public ScheduleRepository(LMCM_DBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool> UpdateSchedulesAsync(List<Schedule> schedules)
        {
            _dbContext.Schedules.UpdateRange(schedules);

            return true;
        }

        public async Task<bool> AddSchedulesAsync(List<Schedule> schedules)
        {
            await _dbContext.Schedules.AddRangeAsync(schedules);

            return true;
        }

        public async Task<List<Schedule>> GetSchedulesBySyllabusAsync(Guid syllabusId)
        {
            var schedules = await _dbContext.Schedules
                .Where(s => s.SyllabusId == syllabusId)
                .ToListAsync();
            return schedules;
        }
    }
}
