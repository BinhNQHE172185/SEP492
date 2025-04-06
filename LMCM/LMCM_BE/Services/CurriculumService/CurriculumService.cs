using AutoMapper;
using LMCM_BE.DTOs.CurriculumDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Models;
using LMCM_BE.Repositories.CurriculumRepository;
using LMCM_BE.Repositories.CurriculumsSubjectRepository;
using LMCM_BE.Repositories.PloRepository;
using LMCM_BE.Repositories.PloSubjectRepository;
using Microsoft.EntityFrameworkCore;

namespace LMCM_BE.Services.CurriculumService
{
    public class CurriculumService : ICurriculumService
    {
        private readonly IMapper _mapper;
        private readonly ICurriculumRepository _curriculumRepository;
        private readonly ICurriculumsSubjectRepository _curriculumSubjectRepository;
        private readonly IPloRepository _ploRepository;
        private readonly IPloSubjectRepository _ploSubjectRepository;

        public CurriculumService(
            IMapper mapper,
            ICurriculumRepository curriculumRepository,
            ICurriculumsSubjectRepository curriculumSubjectRepository,
            IPloRepository ploRepository,
            IPloSubjectRepository ploSubjectRepository
            )
        {
            _mapper = mapper;
            _curriculumRepository = curriculumRepository;
            _curriculumSubjectRepository = curriculumSubjectRepository;
            _ploRepository = ploRepository;
            _ploSubjectRepository = ploSubjectRepository;
        }

        public async Task<PagedResult<CurriculumDto>> GetCurriculumsAsync(string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            var (curriculums, totalCount) = await _curriculumRepository.GetCurriculumsAsync(searchKey, pageIndex, pageSize);
            
            var data = _mapper.Map<List<CurriculumDto>>(curriculums);

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

            await SoftCascadeDeleteCurriculumByCodeAsync(curriculum.CurriculumCode);

            return await _curriculumRepository.ImportCurriculumAsync(curriculum);
        }
        public async Task<bool> SoftDeleteCurriculumAsync(Guid curriculumId)
        {
            var curriculum = await _curriculumRepository.GetActiveCurriculumByIdAsync(curriculumId);
            if (curriculum == null)
                throw new KeyNotFoundException("Không tìm thấy chương trình giảng dạy.");

            return await _curriculumRepository.SoftDeleteCurriculumAsync(curriculum);
        }
        public async Task<bool> SoftCascadeDeleteCurriculumByCodeAsync(string curriculumCode)
        {
            var curriculum = await _curriculumRepository.GetActiveCurriculumByCodeAsync(curriculumCode);
            if (curriculum == null)
                return true;

            await _curriculumSubjectRepository.DeleteCurriculumsSubjectAsync(curriculum.CurriculumId);
            await _ploRepository.DeletePlosAsync(curriculum.Plos.Select(p => p.PloId).ToList());
            await _ploSubjectRepository.DeletePloSubjectsAsync(curriculum.Plos.Select(p => p.PloId).ToList());

            await _curriculumRepository.SoftDeleteCurriculumAsync(curriculum);

            return true;
        }
        public async Task<CurriculumDetailDto?> GetCurriculumDetailAsync(Guid curriculumId)
        {
            var curriculum = await _curriculumRepository.GetCurriculumDetailAsync(curriculumId);
            if (curriculum == null)
                throw new KeyNotFoundException("Không tìm thấy chương trình giảng dạy.");

            return curriculum == null ? null : _mapper.Map<CurriculumDetailDto>(curriculum);
        }
    }
}
