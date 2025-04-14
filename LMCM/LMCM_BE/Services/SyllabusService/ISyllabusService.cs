using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.DTOs.SyllabusDtos;
using LMCM_BE.Models;
using OfficeOpenXml;

namespace LMCM_BE.Services.SyllabusService
{
    public interface ISyllabusService
    {
        Task<PagedResult<SyllabusListViewDto>> GetSyllabusesAsync(string? searchKey, int pageIndex = 1, int pageSize = 10);
        Task<List<SyllabusListViewDto>> GetSyllabusesAsync(string? searchKey);
        Task<PagedResult<SyllabusHistoryList>> GetSyllabusChangeHistoriesAsync(Guid? subjectId, string? searchKey, int pageIndex = 1, int pageSize = 10);
        Task<bool> ImportSyllabusAsync(ExcelWorkbook workbook, bool keepUserCreated);
        Task<bool> DeleteSyllabusAsync(Guid id);
        Task<SyllabusListViewDto?> GetActiveSyllabusBySubjectIdAsync(Guid subjectId);
        Task<SyllabusDetailDto> GetSyllabusDetailAsync(Guid? syllabusId);
        Task<SyllabusListViewDto> GetSyllabusByIdAsync(Guid? syllabusId);
    }
}
