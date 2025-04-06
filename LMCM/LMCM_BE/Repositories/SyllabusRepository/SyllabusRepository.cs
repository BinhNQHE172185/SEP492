using AutoMapper;
using AutoMapper.QueryableExtensions;
using LMCM_BE.DbContext;
using LMCM_BE.DTOs.CLODtos;
using LMCM_BE.DTOs.ConstructivistQuestionDtos;
using LMCM_BE.DTOs.GradingStructureDtos;
using LMCM_BE.DTOs.LearningMaterialDtos;
using LMCM_BE.DTOs.ScheduleDtos;
using LMCM_BE.DTOs.ContractDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.DTOs.SyllabusDtos;
using LMCM_BE.DTOs.UserDtos;
using LMCM_BE.Models;
using LMCM_BE.Repositories.CLORepository;
using LMCM_BE.Repositories.ConstructivistQuestionRepository;
using LMCM_BE.Repositories.GradingStructureRepository;
using LMCM_BE.Repositories.LearningMaterialRepository;
using LMCM_BE.Repositories.ScheduleRepository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace LMCM_BE.Repositories.SyllabusRepository
{
    public class SyllabusRepository : ISyllabusRepository
    {

        private readonly LMCM_DBContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ICLORepository _CLORepository;
        private readonly IConstructivistQuestionRepository _ConstructivistQuestionRepository;
        private readonly IScheduleRepository _ScheduleRepository;
        private readonly ILearningMaterialRepository _LearningMaterialRepository;
        private readonly IGradingStructureRepository _GradingStructureRepository;
        public SyllabusRepository(LMCM_DBContext dbContext, IMapper mapper,ICLORepository cLORepository,
            IConstructivistQuestionRepository constructivistQuestionRepository,IScheduleRepository scheduleRepository,
            ILearningMaterialRepository learningMaterialRepository,IGradingStructureRepository gradingStructureRepository)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _CLORepository = cLORepository;
            _ConstructivistQuestionRepository = constructivistQuestionRepository;
            _ScheduleRepository = scheduleRepository;
            _LearningMaterialRepository = learningMaterialRepository;
            _GradingStructureRepository = gradingStructureRepository;
        }

        public async Task<bool> DeleteSyllabusAsync(Guid id)
        {
            var syllabus = await _dbContext.Syllabus.FindAsync(id);
            if (syllabus == null)
                return false; // Syllabus not found

            syllabus.Status = "Inactive";
            syllabus.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }


        public async Task<PagedResult<SyllabusListViewDto>> GetSyllabusesAsync(string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            var query = _dbContext.Syllabus.AsQueryable();

            query = query.OrderBy(s => s.CourseCode);

            if (!string.IsNullOrWhiteSpace(searchKey))
            {
                string search = searchKey.Trim().ToLower();
                query = query.Where(s => s.CourseCode.ToLower().Contains(search) ||
                                         s.CourseNameEnglish.ToLower().Contains(search) ||
                                         s.CourseName.ToLower().Contains(search));
            }

            query = query.Where(s => s.Status != "Inactive");

            int totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Include(s => s.Subject)
                .ToListAsync();

            var data = _mapper.Map<List<SyllabusListViewDto>>(items);

            return new PagedResult<SyllabusListViewDto>
            {
                Items = data,
                TotalCount = totalCount,
                CurrentPage = pageIndex,
                PageSize = pageSize
            };
        }
        public async Task<List<SyllabusListViewDto>> GetSyllabusesAsync(string? searchKey)
        {
            var query = _dbContext.Syllabus.AsQueryable();

            query = query.OrderBy(s => s.CourseCode);

            query = query.Where(s => s.Status != "Inactive");

            if (!string.IsNullOrWhiteSpace(searchKey))
            {
                string search = searchKey.Trim().ToLower();
                query = query.Where(s => s.CourseCode.ToLower().Contains(search) ||
                                         s.CourseNameEnglish.ToLower().Contains(search) ||
                                         s.CourseName.ToLower().Contains(search));
            }

            var items = await query.ToListAsync();

            var data = _mapper.Map<List<SyllabusListViewDto>>(items);

            return data;
        }
        public async Task<PagedResult<SyllabusListViewDto>> GetSyllabusChangeHistoriesAsync(
            Guid? syllabusId, string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            if (syllabusId == null)
                throw new ArgumentNullException(nameof(syllabusId));

            // Step 1: Get the subject ID from the given syllabus ID
            var subjectId = await _dbContext.Syllabus
                .Where(s => s.SyllabusId == syllabusId)
                .Select(s => s.SubjectId)
                .FirstOrDefaultAsync();

            if (subjectId == Guid.Empty)
                throw new InvalidOperationException("No subject found for the given syllabus ID.");

            // Step 2: Get all syllabuses that belong to the same subject
            var siblingSyllabuses = await _dbContext.Syllabus
                .Where(s => s.SubjectId == subjectId)
                .OrderByDescending(s => s.UpdatedAt ?? s.CreatedAt)
                .ToListAsync();

            int totalCount = siblingSyllabuses.Count;

            var paginatedItems = siblingSyllabuses
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var data = _mapper.Map<List<SyllabusListViewDto>>(paginatedItems);

            return new PagedResult<SyllabusListViewDto>
            {
                Items = data,
                TotalCount = totalCount,
                CurrentPage = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<Syllabus> ImportSyllabusAsync(SyllabusInsertDto syllabus, List<ScheduleInsertDto> schedules,
            List<CLOInsertDto> cLOs, List<GradingStructureInsertDto> gradingStructures,
            List<ConstructivistQuestionInsertDto>? constructivistQuestions,
            List<LearningMaterialImportDto>? learningMaterials, bool keepUserCreated)
        {
            if (syllabus == null)
                throw new ArgumentNullException(nameof(syllabus));

            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var existingSyllabus = await _dbContext.Syllabus
                        .Include(s => s.Clos)
                        .Include(s => s.ConstructivistQuestions)
                        .Include(s => s.Schedules)
                        .Include(s => s.LearningMaterials)
                        .Include(s => s.GradingStructures)
                        .SingleOrDefaultAsync(s => s.CourseCode == syllabus.CourseCode &&
                                                   s.Status != null && s.Status.ToLower() == "active");
                    
                    // Delete old entities
                    if (existingSyllabus != null)
                    {
                        await DeleteSyllabusAsync(existingSyllabus.SyllabusId);
                        await _CLORepository.DeleteCLOBySyllabusAsync(existingSyllabus.SyllabusId);
                        await _ConstructivistQuestionRepository.DeleteConstructivistQuestionsBySyllabusAsync(existingSyllabus.SyllabusId);
                        await _ScheduleRepository.DeleteSchedulesBySyllabusAsync(existingSyllabus.SyllabusId);
                        await _GradingStructureRepository.DeleteGradingStructuresBySyllabusAsync(existingSyllabus.SyllabusId);
                        await _LearningMaterialRepository.DeleteLearningMaterialsBySyllabusAsync(existingSyllabus.SyllabusId);
                    }

                    // Map the incoming syllabus DTO to a new Syllabus entity
                    var newSyllabus = _mapper.Map<Syllabus>(syllabus);
                    newSyllabus.SyllabusId = Guid.NewGuid();
                    newSyllabus.Status = "Active";
                    newSyllabus.CreatedAt = DateTime.UtcNow;
                    newSyllabus.UpdatedAt = DateTime.UtcNow;

                    // Import related entities
                    await _CLORepository.ImportCLOsAsync(cLOs, newSyllabus.SyllabusId);
                    if (constructivistQuestions != null) await _ConstructivistQuestionRepository.ImportConstructivistQuestionsAsync(constructivistQuestions, newSyllabus.SyllabusId);
                    await _ScheduleRepository.ImportSchedulesAsync(schedules, newSyllabus.SyllabusId);
                    await _GradingStructureRepository.ImportGradingStructuresAsync(gradingStructures, newSyllabus.SyllabusId);
                    if(learningMaterials!=null) await _LearningMaterialRepository.ImportLearningMaterialsAsync(learningMaterials, existingSyllabus?.SyllabusId, newSyllabus.SyllabusId, keepUserCreated);

                    // Add the new syllabus to the database
                    await _dbContext.Syllabus.AddAsync(newSyllabus);

                    // Commit the transaction if everything works
                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return newSyllabus;
                }
                catch (Exception ex)
                {
                    // Rollback the transaction in case of an error
                    await transaction.RollbackAsync();
                    throw new InvalidOperationException(ex.Message);
                }
            }
        }



        public async Task<bool> UpdateSyllabusAsync(Syllabus existingSyllabus, SyllabusInsertDto syllabusDto)
        {
            if (existingSyllabus == null)
                throw new ArgumentNullException(nameof(existingSyllabus), "Existing syllabus cannot be null.");

            if (syllabusDto == null)
                throw new ArgumentNullException(nameof(syllabusDto), "Syllabus data cannot be null.");

            // Use AutoMapper to update existing entity
            _mapper.Map(syllabusDto, existingSyllabus);
            existingSyllabus.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<Syllabus?> GetActiveSyllabusBySubjectIdAsync(Guid subjectId)
        {
            var syllabus = await _dbContext.Syllabus
               .Where(s => s.SubjectId == subjectId && s.Status == "Active")
               .FirstOrDefaultAsync();

            return syllabus;
        }

        public async Task<SyllabusDetailDto> GetSyllabusDetailAsync(Guid? syllabusId)
        {
            if (syllabusId == null)
                throw new ArgumentNullException(nameof(syllabusId), "Syllabus ID cannot be null.");
            var syllabusDto = await _dbContext.Syllabus
                .AsNoTracking()
                .Where(s => s.SyllabusId == syllabusId)
                .ProjectTo<SyllabusDetailDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();

            if (syllabusDto == null)
                throw new KeyNotFoundException($"No syllabus found with ID: {syllabusId}");
            return syllabusDto;
        }

        public async Task<Syllabus> GetSyllabusByIdAsync(Guid? syllabusId)
        {
            if (syllabusId == null)
                throw new ArgumentNullException(nameof(syllabusId), "Syllabus ID cannot be null.");
            var syllabus = await _dbContext.Syllabus
                .AsNoTracking()
                .Where(s => s.SyllabusId == syllabusId)
                .SingleOrDefaultAsync();

            if (syllabus == null)
                throw new KeyNotFoundException($"No syllabus found with ID: {syllabusId}");
            return syllabus;
        }
    }
}
