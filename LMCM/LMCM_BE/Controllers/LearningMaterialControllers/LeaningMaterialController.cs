using LMCM_BE.DTOs.AcceptanceRecordDtos;
using LMCM_BE.DTOs.LearningMaterialDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Services.LearningMaterialService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMCM_BE.Controllers.LearningMaterialControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class LearningMaterialController : ControllerBase
    {
        private readonly ILearningMaterialService _learningMaterialService;

        public LearningMaterialController(ILearningMaterialService learningMaterialService)
        {
            _learningMaterialService = learningMaterialService;
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
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
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
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateLearningMaterialAsync([FromBody] LearningMaterialInsertDto material)
        {
            try
            {
                // Check if ModelState is valid (DTO validation will catch issues)
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                                                  .Select(e => e.ErrorMessage)
                                                  .ToList();

                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Dữ liệu đầu vào không hợp lệ.",
                        Errors = errors
                    });
                }
                bool isSuccess = await _learningMaterialService.InsertLearningMaterialAsync(material);
                if (isSuccess)
                    return Ok(new
                    {
                        message = "Thêm thành công."

                    });
                else return BadRequest(new { message = "Thêm thất bại. Vui lòng kiểm tra dữ liệu đầu vào." });
            }
            catch (InvalidDataException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateLearningMaterialAsync(Guid id, [FromBody] LearningMaterialUpdateDto material)
        {
            try
            {
                // Check if ModelState is valid (DTO validation will catch issues)
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                                                  .Select(e => e.ErrorMessage)
                                                  .ToList();

                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Dữ liệu đầu vào không hợp lệ.",
                        Errors = errors
                    });
                }
                bool isSuccess = await _learningMaterialService.UpdateLearningMaterialAsync(id, material);
                if (isSuccess)
                    return Ok(new
                    {
                        message = "Update thành công."

                    });
                else
                {
                    return NotFound(new { message = "Dữ liệu không được tìm thấy." });
                }
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidDataException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
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
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
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
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }
    }
}
