using AutoMapper;
using LMCM_BE.DbContext;
using LMCM_BE.DTOs.CLODtos;
using LMCM_BE.DTOs.ScheduleDtos;
using LMCM_BE.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace LMCM_BE.Repositories.ScheduleRepository
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly LMCM_DBContext _dbContext;
        public ScheduleRepository(LMCM_DBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool> DeleteSchedulesBySyllabusAsync(Guid syllabusId)
        {
            if (syllabusId == Guid.Empty)
                throw new ArgumentException("Syllabus ID cannot be empty.", nameof(syllabusId));

            try
            {
                var schedules = await _dbContext.Schedules
                    .Where(s => s.SyllabusId == syllabusId)
                    .ToListAsync();

                if (!schedules.Any())
                    return false; // No schedules found for the given syllabus

                foreach (var schedule in schedules)
                {
                    schedule.Status = "Inactive";
                    schedule.UpdatedAt = DateTime.UtcNow;
                }

                _dbContext.Schedules.UpdateRange(schedules);

                return true;
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine(dbEx.Message);
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> ImportSchedulesAsync(List<Schedule> schedules, Guid syllabusId)
        {
            if (schedules == null || !schedules.Any())
                throw new ArgumentNullException(nameof(schedules));

            foreach (var schedule in schedules)
            {
                schedule.SyllabusId= syllabusId;    
                schedule.ScheduleId = Guid.NewGuid();
                schedule.Status = "Active";
                schedule.CreatedAt = DateTime.UtcNow;
                schedule.UpdatedAt = DateTime.UtcNow;
            }

            await _dbContext.Schedules.AddRangeAsync(schedules);

            return true;
        }
    }
}
