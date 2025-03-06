using LMCM_BE.DTOs.CLODtos;
using LMCM_BE.Models;

namespace LMCM_BE.Services.CLOService
{
    public interface ICLOService
    {
        Task<bool> ImportCLOsAsync(List<CLOInsertDto> cLOs);
        Task<bool> DeleteCLOBySyllabusAsync(Guid syllabusId);
        Task<bool> UpdateCLOAsync(Clo existingCLO, CLOInsertDto CLODto);
    }
}
