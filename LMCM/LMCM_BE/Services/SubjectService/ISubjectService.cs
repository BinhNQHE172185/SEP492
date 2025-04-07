using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.DTOs.SubjectDtos;
using LMCM_BE.Models;
using OfficeOpenXml;

namespace LMCM_BE.Services.SubjectService
{
    public interface ISubjectService
    {
        Task<PagedResult<SubjectViewDto>> GetSubjectsAsync(string? searchKey, int pageIndex = 1, int pageSize = 10);
        Task<Subject> GetSubjectByCodeAsync(String subjectCode);
        Task<List<Subject>> GetActiveSubjectsByCodesAsync(List<string> subjectCodes);
        Task<bool> ImportSubjectsAsync(ExcelWorksheet worksheet);
        Task<bool> SoftDeleteSubjectAsync(Guid subjectId);
    }
}
