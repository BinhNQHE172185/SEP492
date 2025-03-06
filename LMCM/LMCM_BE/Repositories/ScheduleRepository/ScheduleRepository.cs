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
        private readonly IMapper _mapper;
        public ScheduleRepository(LMCM_DBContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
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
                await _dbContext.SaveChangesAsync();

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

        public async Task<bool> ImportSchedulesAsync(List<ScheduleInsertDto> schedules)
        {
            if (schedules == null || !schedules.Any())
                throw new ArgumentNullException(nameof(schedules));

            var newSchedules = schedules.Select(scheduleDto => new Schedule
            {
                ScheduleId = Guid.NewGuid(),
                SyllabusId = scheduleDto.SyllabusId,
                ScheduleNo = scheduleDto.ScheduleNo,
                Method = scheduleDto.Method,
                Content = scheduleDto.Content,
                Clos = scheduleDto.Clos,
                Itu = scheduleDto.Itu,
                StudentMaterial = scheduleDto.StudentMaterial,
                StudentTask = scheduleDto.StudentTask,
                LecturerMaterial = scheduleDto.LecturerMaterial,
                LecturerTask = scheduleDto.LecturerTask,
                StudentMaterialUrl = scheduleDto.StudentMaterialUrl,
                LecturerMaterialUrl = scheduleDto.LecturerMaterialUrl,
                Status = "Active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }).ToList();

            await _dbContext.Schedules.AddRangeAsync(newSchedules);
            return true;  
        }
    }
}
