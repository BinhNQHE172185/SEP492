using LMCM_BE.DTOs.DashboardDtos;

namespace LMCM_BE.Services.DashboardService
{
    public interface IDashboardService
    {
        Task<ChartDataDto> GetPieChartDataAsync();
        Task<ChartDataDto> GetColumnChartDataAsync();
        Task<ItemsDto> GetItemsAsync();
    }
}
