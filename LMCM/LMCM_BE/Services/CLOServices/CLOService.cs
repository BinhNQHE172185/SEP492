using LMCM_BE.DTOs.CLODtos;
using LMCM_BE.Models;
using LMCM_BE.Repositories.CLORepository;

namespace LMCM_BE.Services.CLOService
{
    public class CLOService : ICLOService
    {
        private readonly ICLORepository _cLORepository;
        public CLOService(ICLORepository cLORepository)
        {
            _cLORepository = cLORepository;
        }

        public async Task<bool> DeleteCLOBySyllabusAsync(Guid syllabusId)
        {
            return await _cLORepository.DeleteCLOBySyllabusAsync(syllabusId);
        }
        public async Task<bool> ImportCLOsAsync(List<CLOInsertDto> cLOs)
        {
            return await _cLORepository.ImportCLOsAsync(cLOs);
        }

        public Task<bool> UpdateCLOAsync(Clo existingCLO, CLOInsertDto CLODto)
        {
            throw new NotImplementedException();
        }
    }
}
