using LMCM_BE.Models;

namespace LMCM_BE.Repositories.CLORepository
{
    public interface ICLORepository
    {
        Task<List<Clo>> GetCLOsBySyllabusASync(Guid syllabusId);
        Task<bool> AddCLOsAsync(List<Clo> cLOs);
        Task<bool> UpdateCLOsAsync(List<Clo> cLOs);
    }
}
