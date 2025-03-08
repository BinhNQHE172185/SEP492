using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.DTOs.SubjectDtos;
using LMCM_BE.Models;

namespace LMCM_BE.Services.SubjectService
{
    public interface ISubjectService
    {
        Task<PagedResult<SubjectViewDto>> GetSubjectsAsync(string? searchKey, int pageIndex = 1, int pageSize = 10);
        Task<bool> InsertSubjectAsync(SubjectInsertDto subject);
        Task<Subject> GetSubjectByCodeAsync(String subjectCode);
        Task<List<Subject>> GetActiveSubjectsByCodesAsync(List<string> subjectCodes);

        Task<bool> ImportSubjectsAsync(List<SubjectInsertDto> subjects);
    }
}
