using LMCM_BE.DTOs.PloDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LMCM_BE.Repositories.PloRepository
{
    public interface IPloRepository
    {
        Task<PagedResult<Plo>> GetPlosAsync(Guid curriculumId, string? searchKey, int pageIndex = 1, int pageSize = 10);
        Task<List<PloDetailDto>> GetPloDetailsByCurriculumIdAsync(Guid curriculumId);
        Task<bool> AddPlosAsync(List<Plo> plos);
        Task<bool> DeletePlosAsync(List<Guid> ploIds);
        Task<bool> HasActivePloAsync(Guid curriculumId);
    }
}
