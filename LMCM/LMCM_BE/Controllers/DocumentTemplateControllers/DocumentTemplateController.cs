using LMCM_BE.DTOs.BudgetProposalDtos;
using LMCM_BE.DTOs.DocumentTemplateDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Services.BudgetPropasalService;
using LMCM_BE.Services.ContractService;
using LMCM_BE.Services.DocumentTemplateService;
using Microsoft.AspNetCore.Mvc;

namespace LMCM_BE.Controllers.DocumentTemplateControllers
{
    [Route("api/documentTemplate")]
    [ApiController]
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
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
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
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    Success = false,
                    Message = "Bạn không có quyền xem mẫu tài liệu.",
                    Error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
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
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    Success = false,
                    Message = "Bạn không có quyền tạo mẫu tài liệu.",
                    Error = ex.Message
                });
            }
            catch (ArgumentException ex) // Handle validation errors
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Dữ liệu đầu vào không hợp lệ.",
                    Error = ex.Message
                });
            }
            catch (Exception ex) // General error handling
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi tạo mẫu tài liệu. Vui lòng thử lại sau.",
                    Error = ex.Message
                });
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
                Guid? templateId = await _documentTemplateService.UpdateTempalteAsync(id, newTemplate);
                if (templateId.HasValue)
                    return Ok(new
                    {
                        message = "Update mẫu tài liệu thành công.",
                        Data = templateId,

                    });
                else
                {
                    return NotFound(new { message = "Dữ liệu không được tìm thấy." });
                }
            }
            catch (UnauthorizedAccessException ex) // Handle permission errors
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    Success = false,
                    Message = "Bạn không có quyền update mẫu tài liệu.",
                    Error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
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
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    Success = false,
                    Message = "Bạn không có quyền xóa mẫu tài liệu.",
                    Error = ex.Message
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
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
