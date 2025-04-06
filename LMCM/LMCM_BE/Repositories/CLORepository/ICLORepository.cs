using LMCM_BE.Models;

namespace LMCM_BE.Repositories.CLORepository
{
    public interface ICLORepository
    {
        Task<bool> ImportCLOsAsync(List<Clo> cLOs,Guid syllabusId);
        Task<bool> DeleteCLOBySyllabusAsync(Guid syllabusId);
    }
}
