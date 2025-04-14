using LMCM_BE.Models;
using LMCM_BE.Shared.Constant;

namespace LMCM_BE.Repositories.SyllabusRepository
{
    public interface ISyllabusRepository
    {
        Task<(List<Syllabus>, int totalCount)> GetSyllabusesAsync(string? searchKey, int pageIndex = 1, int pageSize = 10);
        Task<List<Syllabus>> GetSyllabusesAsync(string? searchKey);
        Task<(List<Syllabus>, int totalCount)> GetSyllabusChangeHistoriesAsync(string courseCode,string? searchKey, int pageIndex = 1, int pageSize = 10);
        Task<bool> ImportSyllabusAsync(Syllabus syllabus);
        Task<Syllabus> GetSyllabusDetailAsync(Guid? syllabusId);
        Task<Syllabus> GetSyllabusByIdAsync(Guid? syllabusId);
        Task<Syllabus> GetActiveSyllabusByIdAsync(Guid? syllabusId);
        Task<Syllabus> GetSyllabusByCourseCodeAsync(string courseCode);
        Task<Syllabus?> GetActiveSyllabusBySubjectIdAsync(Guid subjectId);
        Task<List<Syllabus>> GetSyllabusesBySubjectIdAsync(Guid subjectId);
        Task<bool> UpdateSyllabusAsync(Syllabus syllabus);
        Task<int> CountSyllabusByStatus(GenericStatus status);
    }
}
