using LMCM_BE.Models;

namespace LMCM_BE.Repositories.SubjectRepository.SubjectRepository
{
    public interface ISubjectRepository
    {
        Task<(List<Subject>,int totalCount)> GetSubjectsAsync(string? searchKey, int pageIndex = 1, int pageSize = 10);
        Task<Subject> GetSubjectByCodeAsync(String subjectCode);
        Task<Subject> GetSubjectByIdAsync(Guid subjectId);
        Task<List<Subject>> GetActiveSubjectsByCodesAsync(List<string> subjectCodes);
        Task<Dictionary<string, Subject>> GetSubjectsDictonaryAsync();
        Task<bool> AddSubjectsAsync(List<Subject> subjects);
        Task<bool> UpdateSubjectsAsync(List<Subject> subjects);
        Task<bool> UpdateSubjectAsync(Subject subject);
    }
}
