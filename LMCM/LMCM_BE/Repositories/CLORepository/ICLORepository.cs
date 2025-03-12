using LMCM_BE.DTOs.CLODtos;
using LMCM_BE.DTOs.SubjectDtos;
using LMCM_BE.DTOs.SyllabusDtos;
using LMCM_BE.Models;

namespace LMCM_BE.Repositories.CLORepository
{
    public interface ICLORepository
    {
        Task<bool> ImportCLOsAsync(List<CLOInsertDto> cLOs);
        Task<bool> DeleteCLOBySyllabusAsync(Guid syllabusId);
    }
}
