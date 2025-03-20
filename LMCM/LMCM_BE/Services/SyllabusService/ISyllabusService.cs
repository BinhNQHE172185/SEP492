using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.DTOs.SyllabusDtos;
using LMCM_BE.Models;

namespace LMCM_BE.Services.SyllabusService
{
    public interface ISyllabusService
    {
        Task<PagedResult<SyllabusListViewDto>> GetSyllabusesAsync(string? searchKey, int pageIndex = 1, int pageSize = 10);
        Task<PagedResult<SyllabusChangesHistoryListDto>> GetSyllabusChangeHistoriesAsync(Guid? syllabusId, string? searchKey, int pageIndex = 1, int pageSize = 10);
        Task<Syllabus> ImportSyllabusAsync(SyllabusInsertDto syllabus);
        Task<bool> DeleteSyllabusAsync(Guid id);
        Task<bool> HasActiveSyllabusesBySubjectIdAsync(Guid subjectId);
        Task<SyllabusDetailDto> GetSyllabusDetailAsync(Guid? syllabusId);
        Task<Syllabus> GetSyllabusByIdAsync(Guid? syllabusId);
    }
}
