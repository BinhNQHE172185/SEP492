using LMCM_BE.Models;

namespace LMCM_BE.Repositories.SubjectRepository.SubjectRepository
{
    public interface ISubjectRepository
    {
        Task<(List<Subject>,int totalCount)> GetSubjectsAsync(string? searchKey, int pageIndex = 1, int pageSize = 10);
        Task<Subject> GetSubjectByCodeAsync(String subjectCode);
        Task<Subject> GetSubjectByIdAsync(Guid subjectId);
        Task<List<Subject>> GetActiveSubjectsByCodesAsync(List<string> subjectCodes);
        Task<bool> UpdateSubjectIfChangedAsync(Subject existingSubject, Subject newSubject);
        Task<bool> ImportSubjectsAsync(List<Subject> subjects);
        Task<bool> SoftDeleteSubjectAsync(Subject subject);
    }
}
