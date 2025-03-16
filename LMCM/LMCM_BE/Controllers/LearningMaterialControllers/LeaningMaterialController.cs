using LMCM_BE.DTOs.LearningMaterialDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Services.LearningMaterialService;
using Microsoft.AspNetCore.Mvc;

namespace LMCM_BE.Controllers.LearningMaterialControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LearningMaterialController : ControllerBase
    {
        private readonly ILearningMaterialService _learningMaterialService;
        private readonly ILearningMaterialChangesHistorySerivce _changesService;

        public LearningMaterialController(
            ILearningMaterialService learningMaterialService,
            ILearningMaterialChangesHistorySerivce changesService)
        {
            _learningMaterialService = learningMaterialService;
            _changesService = changesService;
        }

        [HttpPost("getPagedMaterialsList")]
        public async Task<IActionResult> GetMaterialsAsync([FromBody] PagingRequest request)
        {
            try
            {
                var data = await _learningMaterialService.GetMaterialsBySyllabusIdAsync((Guid)request.Id,request.SearchKey, request.pageIndex, request.PageSize);
                if (data != null)
                {
                    return Ok(data);
                }
                return NotFound(new { message = "Dữ liệu không được tìm thấy." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("getMaterialsList")]
        public async Task<IActionResult> GetMaterialsAsync(Guid syllabusId)
        {
            try
            {
                var data = await _learningMaterialService.GetMaterialsBySyllabusIdAsync(syllabusId);
                if (data != null)
                {
                    return Ok(data);
                }
                return NotFound(new { message = "Dữ liệu không được tìm thấy." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLearningMaterialByIdAsync(Guid id)
        {
            try
            {
                var material = await _learningMaterialService.GetLearningMaterialByIdAsync(id);
                if (material == null)
                {
                    return NotFound(new { message = "Dữ liệu không được tìm thấy." });
                }
                return Ok(material);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateLearningMaterialAsync([FromBody] LearningMaterialInsertDto material)
        {
            try
            {
                var createdMaterial = await _learningMaterialService.InsertLearningMaterialAsync(material);
                return Ok(createdMaterial);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateLearningMaterialAsync(Guid id, [FromBody] LearningMaterialUpdateDto material)
        {
            try
            {
                var updatedMaterial = await _learningMaterialService.UpdateLearningMaterialAsync(id, material);
                if (updatedMaterial == null)
                {
                    return NotFound(new { message = "Dữ liệu không được tìm thấy." });
                }
                return Ok(updatedMaterial);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteLearningMaterialAsync(Guid id)
        {
            try
            {
                var success = await _learningMaterialService.DeleteLearningMaterialByIdAsync(id);
                if (!success)
                {
                    return NotFound(new { message = "Dữ liệu không được tìm thấy." });
                }
                return Ok(new { message = "Xóa tài liệu thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("getChangesHistoryList")]
        public async Task<IActionResult> GetChangesHistoriesAsync([FromBody] PagingRequest request)
        {
            try
            {
                var data = await _changesService.GetChangesHistoriesAsync(request.SearchKey, request.pageIndex, request.PageSize);
                if (data != null)
                {
                    return Ok(data);
                }
                return NotFound(new { message = "Dữ liệu không được tìm thấy." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
