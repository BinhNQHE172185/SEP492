using LMCM_BE.DTOs.DocumentTemplateDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Services.DocumentTemplateService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMCM_BE.Controllers.DocumentTemplateControllers
{
    [Route("api/documentTemplate")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DocumentTemplateController : Controller
    {
        private readonly IDocumentTemplateService _documentTemplateService;

        public DocumentTemplateController(IDocumentTemplateService documentTemplateService)
        {
            _documentTemplateService = documentTemplateService;
        }

        [HttpPost("getTemplatesList")]
        public async Task<IActionResult> GetTemplatesAsync([FromBody] PagingRequest request)
        {
            try
            {
                var data = await _documentTemplateService.GetTemplatesAsync(request.SearchKey, request.pageIndex, request.PageSize);
                if (data != null)
                {
                    return Ok(data);
                }
                return NotFound(new { message = "Dữ liệu không được tìm thấy." });
            }
            catch (UnauthorizedAccessException ex) // Handle permission errors
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Lỗi: " + ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }
        [HttpGet("getTemplateDetail")]
        public async Task<IActionResult> GetTemplateDetailAsync(Guid templateId)
        {
            try
            {
                var data = await _documentTemplateService.GetTemplateByIdAsync(templateId);
                if (data != null)
                {
                    return Ok(data);
                }
                return NotFound(new { message = "Dữ liệu không được tìm thấy." });
            }
            catch (UnauthorizedAccessException ex) // Handle permission errors
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Lỗi: " + ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }
        [HttpPost("createTemplate")]
        public async Task<IActionResult> CreateTemplateAsync([FromForm] DocumentTemplateInsertDto templateDto)
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

                var template = await _documentTemplateService.CreateTemplatelAsync(templateDto);
                return Ok(new
                {
                    Success = true,
                    Message = "Mẫu tài liệu đã được tạo thành công."
                });
            }
            catch (UnauthorizedAccessException ex) // Handle permission errors
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Lỗi: " + ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }
        [HttpPut("updateTemplate/{id}")]
        public async Task<IActionResult> UpdateTemplateAsync(Guid id, [FromForm] DocumentTemplateUpdateDto newTemplate)
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
                bool isSuccess = await _documentTemplateService.UpdateTempalteAsync(id, newTemplate);
                if (isSuccess)
                    return Ok(new
                    {
                        message = "Update mẫu tài liệu thành công."

                    });
                else
                {
                    return NotFound(new { message = "Update mẫu tài liệu thất bại." });
                }
            }
            catch (UnauthorizedAccessException ex) // Handle permission errors
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Lỗi: " + ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }
        [HttpDelete("deleteTemplate/{templateId}")]
        public async Task<IActionResult> DeleteTemplateASync(Guid templateId)
        {
            try
            {
                var result = await _documentTemplateService.SoftDeleteTemplateAsync(templateId);
                return result ? Ok(new { message = "Xóa thành công." }) : NotFound(new { message = "Không tìm thấy ." });
            }
            catch (UnauthorizedAccessException ex) // Handle permission errors
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Lỗi: " + ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }
    }
}
