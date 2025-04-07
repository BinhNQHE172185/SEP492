using LMCM_BE.DTOs.CurriculumDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Models;
using System.Threading.Tasks;

namespace LMCM_BE.Repositories.CurriculumRepository
{
    public interface ICurriculumRepository
    {
        Task<(List<Curriculum>, int totalCount)> GetCurriculumsAsync(string? searchKey, int pageIndex = 1, int pageSize = 10);
        Task<bool> ImportCurriculumAsync(Curriculum curriculum);
        Task<bool> SoftDeleteCurriculumAsync(Curriculum curriculum);
        Task<Curriculum?> GetCurriculumDetailAsync(Guid curriculumId);
        Task<Curriculum?> GetActiveCurriculumByCodeAsync(string curriculumCode);
        Task<Curriculum?> GetActiveCurriculumByIdAsync(Guid curriculumId);
    }
}
