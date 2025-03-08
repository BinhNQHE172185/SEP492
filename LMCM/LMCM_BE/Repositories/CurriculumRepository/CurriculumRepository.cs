using AutoMapper;
using LMCM_BE.DbContext;
using LMCM_BE.DTOs.CurriculumDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Models;
using LMCM_BE.Repositories.CurriculumsSubjectRepository;
using LMCM_BE.Repositories.PloRepository;
using LMCM_BE.Repositories.PloSubjectRepository;
using Microsoft.EntityFrameworkCore;

namespace LMCM_BE.Repositories.CurriculumRepository
{
    public class CurriculumRepository : ICurriculumRepository
    {
        private readonly LMCM_DBContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ICurriculumsSubjectRepository _curriculumSubjectRepository;
        private readonly IPloRepository _ploRepository;
        private readonly IPloSubjectRepository _ploSubjectRepository;

        public CurriculumRepository(
            LMCM_DBContext dbContext,
            IMapper mapper,
            ICurriculumsSubjectRepository curriculumSubjectRepository,
            IPloRepository ploRepository,
            IPloSubjectRepository ploSubjectRepository)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _curriculumSubjectRepository = curriculumSubjectRepository;
            _ploRepository = ploRepository;
            _ploSubjectRepository = ploSubjectRepository;
        }

        public async Task<PagedResult<CurriculumDto>> GetCurriculumsAsync(string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            var query = _dbContext.Curriculums.Include(c => c.CurriculumsSubjects).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchKey))
            {
                string search = searchKey.Trim().ToLower();
                query = query.Where(c => c.CurriculumCode.ToLower().Contains(search) ||
                                         c.CurriculumName.ToLower().Contains(search));
            }

            int totalCount = await query.CountAsync();

            var items = await query.Skip((pageIndex - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            var data = _mapper.Map<List<CurriculumDto>>(items);

            return new PagedResult<CurriculumDto>
            {
                Items = data,
                TotalCount = totalCount,
                CurrentPage = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<bool> ImportCurriculumAsync(Curriculum curriculum)
        {
            if (curriculum == null)
                throw new ArgumentNullException(nameof(curriculum));

            try
            {
                using var transaction = await _dbContext.Database.BeginTransactionAsync();

                // Step 1: Get existing active curriculum using curriculum code
                var existingCurriculum = await _dbContext.Curriculums
                    .Include(c => c.Plos)
                    .Include(c => c.CurriculumsSubjects)
                    .Where(c => c.CurriculumCode == curriculum.CurriculumCode && c.Status == "Active")
                    .FirstOrDefaultAsync();


                if (existingCurriculum != null)
                {
                    // Step 2: Delete related data
                    // Soft delete existing curriculum by updating its status
                    existingCurriculum.Status = "Inactive";
                    existingCurriculum.UpdatedAt = DateTime.UtcNow;
                    _dbContext.Curriculums.Update(existingCurriculum);

                    await _curriculumSubjectRepository.DeleteCurriculumsSubjectAsync(existingCurriculum.CurriculumId);
                    await _ploRepository.DeletePlosAsync(existingCurriculum.Plos.Select(p => p.PloId).ToList());
                    await _ploSubjectRepository.DeletePloSubjectsAsync(existingCurriculum.Plos.Select(p => p.PloId).ToList());
                }

                // Step 3: Insert the new curriculum
                _dbContext.Curriculums.Add(curriculum);
                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }


    }
}
