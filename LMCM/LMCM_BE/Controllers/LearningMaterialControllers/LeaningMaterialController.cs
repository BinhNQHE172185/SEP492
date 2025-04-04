using LMCM_BE.DTOs.AcceptanceRecordDtos;
using LMCM_BE.DTOs.LearningMaterialDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.DTOs.UserDtos;
using LMCM_BE.Models;
using LMCM_BE.Services.ContractService;
using LMCM_BE.Services.LearningMaterialChangesHistoryService;
using LMCM_BE.Services.LearningMaterialService;
using LMCM_BE.Services.SyllabusService;
using LMCM_BE.Services.UserService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LMCM_BE.Controllers.LearningMaterialControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LearningMaterialController : ControllerBase
    {
        private readonly ILearningMaterialService _learningMaterialService;
        private readonly ISyllabusService _syllabusService;

        public LearningMaterialController(
            ILearningMaterialService learningMaterialService,
            ISyllabusService syllabusService
            )
        {
            _learningMaterialService = learningMaterialService;
            _syllabusService = syllabusService;
        }

        [HttpPost("getPagedMaterialsList")]
        public async Task<IActionResult> GetMaterialsAsync([FromBody] PagingRequest request)
        {
            try
            {
                var data = await _learningMaterialService.GetMaterialsBySyllabusIdAsync((Guid)request.Id, request.SearchKey, request.pageIndex, request.PageSize);
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
                var syllabus=await _syllabusService.GetSyllabusByIdAsync(material.SyllabusId);
                if (syllabus == null) BadRequest(new { message = "Không tìm thấy syllabus." });
                Guid? materialId = await _learningMaterialService.InsertLearningMaterialAsync(material);
                if (materialId.HasValue)
                    return Ok(new
                    {
                        message = "Thêm thành công.",
                        Data = materialId,

                    });
                else return BadRequest(new { message = "Thêm thất bại. Vui lòng kiểm tra dữ liệu đầu vào." });
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
                Guid? materialId = await _learningMaterialService.UpdateLearningMaterialAsync(id, material);
                if (materialId.HasValue)
                    return Ok(new
                    {
                        message = "Update thành công.",
                        Data = materialId,

                    });
                else
                {
                    return NotFound(new { message = "Dữ liệu không được tìm thấy." });
                }
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

        [HttpGet("getPublishersList")]
        public async Task<IActionResult> GetPublishersAsync()
        {
            try
            {
                var data = await _learningMaterialService.GetPublishersAsync();
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
