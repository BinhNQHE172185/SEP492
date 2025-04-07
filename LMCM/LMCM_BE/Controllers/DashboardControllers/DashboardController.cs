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

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("pie-chart")]
        public async Task<IActionResult> GetPieChartData()
        {
            var result = await _dashboardService.GetPieChartDataAsync();
            return Ok(result);
        }

        [HttpGet("column-chart")]
        public async Task<IActionResult> GetColumnChartData()
        {
            var result = await _dashboardService.GetColumnChartDataAsync();
            return Ok(result);
        }
    }
}
