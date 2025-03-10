using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.DTOs.SubjectDtos;
using LMCM_BE.DTOs.SyllabusDtos;
using LMCM_BE.Models;

namespace LMCM_BE.Repositories.SyllabusRepository
{
    public interface ISyllabusRepository
    {
        Task<PagedResult<SyllabusListViewDto>> GetSyllabusesAsync(string? searchKey, int pageIndex = 1, int pageSize = 10);
        Task<Syllabus> ImportSyllabusAsync(SyllabusInsertDto syllabus);
        Task<bool> UpdateSyllabusAsync(Syllabus existingSyllabus, SyllabusInsertDto syllabusDto);
        Task<bool> DeleteSyllabusAsync(Guid id);
        Task<bool> HasActiveSyllabusesBySubjectIdAsync(Guid subjectId);
    }
}
