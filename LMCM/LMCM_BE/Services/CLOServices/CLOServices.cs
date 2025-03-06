using LMCM_BE.DTOs.CLODtos;
using LMCM_BE.DTOs.SubjectDtos;
using LMCM_BE.Models;
using LMCM_BE.Repositories.CLORepository;
using LMCM_BE.Repositories.SubjectRepository.SubjectRepository;

namespace LMCM_BE.Services.CLOServices
{
    public class CLOServices : ICLOServices
    {
        private readonly ICLORepository _cLORepository;
        public CLOServices(ICLORepository cLORepository)
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
