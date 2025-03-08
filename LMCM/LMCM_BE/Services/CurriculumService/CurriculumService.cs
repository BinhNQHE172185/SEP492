using LMCM_BE.DTOs.CurriculumDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Models;
using LMCM_BE.Repositories.CurriculumRepository;

namespace LMCM_BE.Services.CurriculumService
{
    public class CurriculumService : ICurriculumService
    {
        private readonly ICurriculumRepository _curriculumRepository;

        public CurriculumService(ICurriculumRepository curriculumRepository)
        {
            _curriculumRepository = curriculumRepository;
        }

        public async Task<PagedResult<CurriculumDto>> GetCurriculumsAsync(string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            return await _curriculumRepository.GetCurriculumsAsync(searchKey, pageIndex, pageSize);
        }


        public async Task<bool> ImportCurriculumAsync(Curriculum curriculum)
        {
            return await _curriculumRepository.ImportCurriculumAsync(curriculum);
        }
    }
}
