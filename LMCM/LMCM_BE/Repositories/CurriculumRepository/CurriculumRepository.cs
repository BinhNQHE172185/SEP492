using AutoMapper;
using LMCM_BE.DbContext;
using LMCM_BE.DTOs.CurriculumDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Models;
using Microsoft.EntityFrameworkCore;

namespace LMCM_BE.Repositories.CurriculumRepository
{
    public class CurriculumRepository : ICurriculumRepository
    {
        private readonly LMCM_DBContext _dbContext;
        private readonly IMapper _mapper;

        public CurriculumRepository(LMCM_DBContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
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

        public async Task<bool> InsertCurriculum(CurriculumDto curriculumDto)
        {
            if (curriculumDto == null) throw new ArgumentNullException(nameof(curriculumDto));

            try
            {
                var curriculum = new Curriculum
                {
                    CurriculumId = Guid.NewGuid(),
                    CurriculumCode = curriculumDto.CurriculumCode,
                    CurriculumName = curriculumDto.Name,
                    CurriculumDescription = curriculumDto.Description,
                    DecisionNo = curriculumDto.DecisionNo,
                    ApprovedDate = curriculumDto.ApprovedDate,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _dbContext.Curriculums.AddAsync(curriculum);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> ImportCurriculumsAsync(List<Curriculum> curriculums)
        {
            if (curriculums == null || curriculums.Count == 0)
                throw new ArgumentNullException(nameof(curriculums));

            try
            {
                var existingCurriculums = await _dbContext.Curriculums
                    .ToDictionaryAsync(c => c.CurriculumCode);

                var newCurriculums = new List<Curriculum>();
                var updatedCurriculums = new List<Curriculum>();

                foreach (var curriculum in curriculums)
                {
                    if (existingCurriculums.TryGetValue(curriculum.CurriculumCode, out var existingCurriculum))
                    {
                        bool isUpdated = false;

                        if (existingCurriculum.CurriculumName != curriculum.CurriculumName)
                        {
                            existingCurriculum.CurriculumName = curriculum.CurriculumName;
                            isUpdated = true;
                        }
                        if (existingCurriculum.CurriculumNameEnglish != curriculum.CurriculumNameEnglish)
                        {
                            existingCurriculum.CurriculumNameEnglish = curriculum.CurriculumNameEnglish;
                            isUpdated = true;
                        }
                        if (existingCurriculum.CurriculumDescription != curriculum.CurriculumDescription)
                        {
                            existingCurriculum.CurriculumDescription = curriculum.CurriculumDescription;
                            isUpdated = true;
                        }
                        if (existingCurriculum.VocationalCode != curriculum.VocationalCode)
                        {
                            existingCurriculum.VocationalCode = curriculum.VocationalCode;
                            isUpdated = true;
                        }
                        if (existingCurriculum.VocationalName != curriculum.VocationalName)
                        {
                            existingCurriculum.VocationalName = curriculum.VocationalName;
                            isUpdated = true;
                        }
                        if (existingCurriculum.EnglishVocationalName != curriculum.EnglishVocationalName)
                        {
                            existingCurriculum.EnglishVocationalName = curriculum.EnglishVocationalName;
                            isUpdated = true;
                        }
                        if (existingCurriculum.DecisionNo != curriculum.DecisionNo)
                        {
                            existingCurriculum.DecisionNo = curriculum.DecisionNo;
                            isUpdated = true;
                        }
                        if (existingCurriculum.ApprovedDate != curriculum.ApprovedDate)
                        {
                            existingCurriculum.ApprovedDate = curriculum.ApprovedDate;
                            isUpdated = true;
                        }
                        if (existingCurriculum.Status != curriculum.Status)
                        {
                            existingCurriculum.Status = curriculum.Status;
                            isUpdated = true;
                        }

                        if (isUpdated)
                        {
                            existingCurriculum.UpdatedAt = DateTime.UtcNow;
                            updatedCurriculums.Add(existingCurriculum);
                        }
                    }
                    else
                    {
                        newCurriculums.Add(new Curriculum
                        {
                            CurriculumId = Guid.NewGuid(),
                            CurriculumCode = curriculum.CurriculumCode,
                            CurriculumName = curriculum.CurriculumName,
                            CurriculumNameEnglish = curriculum.CurriculumNameEnglish,
                            CurriculumDescription = curriculum.CurriculumDescription,
                            VocationalCode = curriculum.VocationalCode,
                            VocationalName = curriculum.VocationalName,
                            EnglishVocationalName = curriculum.EnglishVocationalName,
                            DecisionNo = curriculum.DecisionNo,
                            ApprovedDate = curriculum.ApprovedDate,
                            Status = curriculum.Status,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        });
                    }
                }

                if (updatedCurriculums.Any())
                    _dbContext.Curriculums.UpdateRange(updatedCurriculums);

                if (newCurriculums.Any())
                    await _dbContext.Curriculums.AddRangeAsync(newCurriculums);

                if (updatedCurriculums.Any() || newCurriculums.Any())
                    await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (DbUpdateException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
