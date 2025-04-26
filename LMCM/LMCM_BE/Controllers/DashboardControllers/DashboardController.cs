using LMCM_BE.Services.DashboardService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LMCM_BE.Controllers.DashboardControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(IDashboardService dashboardService,
                                   ILogger<DashboardController> logger)
        {
            _dashboardService = dashboardService;
            _logger = logger;
        }

        [HttpGet("pie-chart")]
        public async Task<IActionResult> GetPieChartData()
        {
            try
            {
                var result = await _dashboardService.GetPieChartDataAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu pie‑chart");
                return StatusCode(500, "Đã xảy ra lỗi hệ thống.");
            }
        }

        [HttpGet("column-chart")]
        public async Task<IActionResult> GetColumnChartData()
        {
            try
            {
                var result = await _dashboardService.GetColumnChartDataAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu column‑chart");
                return StatusCode(500, "Đã xảy ra lỗi hệ thống.");
            }
        }

        [HttpGet("items")]
        public async Task<IActionResult> GetItems()
        {
            try
            {
                var result = await _dashboardService.GetItemsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách items");
                return StatusCode(500, "Đã xảy ra lỗi hệ thống.");
            }
        }
    }
}
