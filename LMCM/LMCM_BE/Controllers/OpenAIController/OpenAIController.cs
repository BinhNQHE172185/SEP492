using LMCM_BE.Services.OpenAIService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static LMCM_BE.DTOs.OpenAIDtos.OpenAIDto;

namespace LMCM_BE.Controllers.OpenAIController
{
    [Route("api/[controller]")]
    [ApiController]
    public class OpenAIController : ControllerBase
    {
        private readonly IOpenAIService _openAIService;

        public OpenAIController(IOpenAIService openAiService)
        {
            _openAIService = openAiService;
        }

        [HttpPost("analyze-upload-file")]
        public async Task<IActionResult> AnalyzeUploadFile([FromForm] AnalyzeRequest request)
        {
            if (request.File == null || request.File.Length == 0)
            {
                return BadRequest(new
                {
                    message = "File is required."
                });
            }

            try
            {
                using var stream = request.File.OpenReadStream();
                var result = await _openAIService.UploadAndAnalyzeContractFileAsync(stream, request.File.FileName, request.Prompt);

                return Ok(new
                {
                    message = "Đọc dữ liệu thành công.",
                    result
                });
            }
            catch (Exception ex)
            {
                // Ghi log nếu cần: _logger.LogError(ex, "Error analyzing file");
                return StatusCode(500, new
                {
                    message = "Phân tích thất bại. Vui lòng thử lại sau.",
                    error = ex.Message
                });
            }
        }
        [HttpPost("analyze-upload-record-file")]
        public async Task<IActionResult> AnalyzeUploadRecordFile([FromForm] AnalyzeRequest request)
        {
            if (request.File == null || request.File.Length == 0)
            {
                return BadRequest(new
                {
                    message = "File is required."
                });
            }

            try
            {
                using var stream = request.File.OpenReadStream();
                var result = await _openAIService.UploadAndAnalyzeRecordFileAsync(stream, request.File.FileName, request.Prompt);

                return Ok(new
                {
                    message = "Đọc dữ liệu thành công.",
                    result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Phân tích thất bại. Vui lòng thử lại sau.",
                    error = ex.Message
                });
            }
        }
    }
}
