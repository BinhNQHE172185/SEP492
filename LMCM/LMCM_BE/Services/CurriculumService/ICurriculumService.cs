using LMCM_BE.DTOs.CurriculumDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Models;
using OfficeOpenXml;

namespace LMCM_BE.Services.CurriculumService
{
    public interface ICurriculumService
    {
        Task<PagedResult<CurriculumDto>> GetCurriculumsAsync(string? searchKey, int pageIndex = 1, int pageSize = 10);
        Task<bool> ImportCurriculumAsync(Curriculum curriculum);
        Task<bool> SoftDeleteCurriculumAsync(Guid curriculumId);
        Task<bool> SoftCascadeDeleteCurriculumByCodeAsync(string curriculumCode);
        Task<CurriculumDetailDto?> GetCurriculumDetailAsync(Guid curriculumId);
        Task<bool> ValidateSheets(ExcelWorkbook workbook, Dictionary<string, List<(string Header, string Cell)>> expectedHeaders);
        Task<bool> ImportCurriculumFromWorkbookAsync(ExcelWorkbook workbook, Dictionary<string, List<(string Header, string Cell)>> expectedHeaders);
    }
}
