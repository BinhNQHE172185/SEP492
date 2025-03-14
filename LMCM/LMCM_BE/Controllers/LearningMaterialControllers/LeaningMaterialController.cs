using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Services.CurriculumService;
using LMCM_BE.Services.LearningMaterialService;
using LMCM_BE.Services.SubjectService;
using Microsoft.AspNetCore.Mvc;

namespace LMCM_BE.Controllers.LearningMaterialControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LearningMaterialController : ControllerBase
    {
        private readonly ILearningMaterialChangesHistorySerivce _changesService;

        public LearningMaterialController(ILearningMaterialChangesHistorySerivce changesService)
        {
            _changesService = changesService;
        }

        [HttpGet("getChangesHistoryList")]
        public async Task<IActionResult> GetChangesHistoriesAsync([FromQuery] PagingRequest request)
        {
            try
            {
                var data = await _changesService.GetChangesHistoriesAsync(request.SearchKey, request.pageIndex, request.PageSize);
                if (data != null)
                {
                    return Ok(data);
                }
                return NotFound(new { message = "Data not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
