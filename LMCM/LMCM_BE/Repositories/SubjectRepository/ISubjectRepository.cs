using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.DTOs.SubjectDtos;
using LMCM_BE.Models;

namespace LMCM_BE.Repositories.SubjectRepository.SubjectRepository
{
    public interface ISubjectRepository
    {
        Task<PagedResult<SubjectViewDto>> GetSubjectsAsync(string? searchKey, int pageIndex = 1, int pageSize = 10);
        Task<Subject> GetSubjectByCodeAsync(String subjectCode);
        Task<Subject> GetSubjectByIdAsync(Guid subjectId);
        Task<List<Subject>> GetActiveSubjectsByCodesAsync(List<string> subjectCodes);
        Task<bool> InsertSubjectAsync(SubjectInsertDto subject);
        Task<bool> UpdateSubjectIfChangedAsync(Subject existingSubject, SubjectInsertDto subjectDto);
        Task<bool> ImportSubjectsAsync(List<SubjectInsertDto> subjects);
        Task<bool> SoftDeleteSubjectAsync(Guid subjectId);
    }
}
