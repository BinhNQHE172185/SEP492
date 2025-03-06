using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.DTOs.SubjectDtos;
using LMCM_BE.DTOs.SyllabusDtos;

namespace LMCM_BE.Repositories.SyllabusRepository
{
    public interface ISyllabusRepository
    {
        Task<PagedResult<SyllabusListViewDto>> GetSyllabusesAsync(string? searchKey, int pageIndex = 1, int pageSize = 10);
        Task<bool> ImportSyllabusAsync(SyllabusInsertDto syllabus);
    }
}
