using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.DTOs.SubjectDtos;
using LMCM_BE.DTOs.SyllabusDtos;
using LMCM_BE.Models;
using LMCM_BE.Repositories.SubjectRepository.SubjectRepository;
using LMCM_BE.Repositories.SyllabusRepository;

namespace LMCM_BE.Services.SyllabusService
{
    public class SyllabusService:ISyllabusService
    {
        private readonly ISyllabusRepository _syllabusRepository;
        public SyllabusService(ISyllabusRepository syllabusRepository)
        {
            _syllabusRepository = syllabusRepository;
        }

        public async Task<bool> DeleteSyllabusAsync(Guid id)
        {
            return await _syllabusRepository.DeleteSyllabusAsync(id);
        }

        public async Task<PagedResult<SyllabusListViewDto>> GetSyllabusesAsync(string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            var data = await _syllabusRepository.GetSyllabusesAsync(searchKey, pageIndex, pageSize);
            return data;
        }

        public async Task<bool> ImportSyllabusAsync(SyllabusInsertDto syllabus)
        {
            return await _syllabusRepository.ImportSyllabusAsync(syllabus);
        }
    }
}
