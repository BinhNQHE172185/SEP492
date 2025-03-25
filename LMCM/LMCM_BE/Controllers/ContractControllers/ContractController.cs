using LMCM_BE.DTOs.BudgetProposalDtos;
using LMCM_BE.DTOs.ContractDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Repositories.ContractRepository;
using LMCM_BE.Services.AcceptanceRecordService;
using LMCM_BE.Services.ContractService;
using Microsoft.AspNetCore.Mvc;

namespace LMCM_BE.Controllers.ContractControllers
{
    [Route("api/contracts")]
    [ApiController]
    public class ContractController : ControllerBase
    {
        private readonly IContractService _contractService;
        private readonly IAcceptanceRecordService _acceptanceRecordService;

        public ContractController(IContractService contractService,IAcceptanceRecordService acceptanceRecordService)
        {
            _contractService = contractService;
            _acceptanceRecordService = acceptanceRecordService; 
        }

        [HttpPost("getContractList")]
        public async Task<IActionResult> GetContractsAsync([FromBody] PagingRequest request)
        {
            try
            {
                var data = await _contractService.GetContractsAsync(request.Id, request.SearchKey, request.pageIndex, request.PageSize);
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
        [HttpPost("createContract")]
        public async Task<IActionResult> CreateContractAsync([FromForm] ContractInsertDto contractDto)
        {
            try
            {
                if (contractDto.File == null)
                {
                    return BadRequest(new { Success = false, Message = "Không tìm thấy file." });
                }
                // Check file type
                if (contractDto.File.ContentType != "application/pdf")
                {
                    return BadRequest(new { Success = false, Message = "Chỉ file pdf mới được tải lên." });
                }

                // Check file size (5MB = 5 * 1024 * 1024 bytes)
                if (contractDto.File.Length > 5 * 1024 * 1024)
                {
                    return BadRequest(new { Success = false, Message = "Dung lượng file không được vượt quá 5MB." });
                }
                var contract = await _contractService.CreateContract(contractDto);
                return Ok(new
                {
                    Success = true,
                    Message = "Hợp đồng đã được tạo thành công."
                });
            }
            catch (UnauthorizedAccessException ex) // Handle permission errors
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    Success = false,
                    Message = "Bạn không có quyền tạo hợp đồng.",
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
                    Message = "Đã xảy ra lỗi khi tạo hợp đồng. Vui lòng thử lại sau.",
                    Error = ex.Message
                });
            }
        }
        [HttpGet("getContractDetail")]
        public async Task<IActionResult> GetContractDetailAsync(Guid contractId,Guid userId)
        {
            try
            {
                var data = await _contractService.GetContractByIdAsync(contractId, userId);
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
                    Message = "Bạn không có quyền xem hợp đồng.",
                    Error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpPut("updateContract/{id}")]
        public async Task<IActionResult> UpdateContractAsync(Guid id, [FromForm] ContractUpdateDto newContract)
        {
            try
            {
                Guid? contractId = await _contractService.UpdateContractAsync(id, newContract);
                if (contractId.HasValue)
                    return Ok(new
                    {
                        message = "Update thành công.",
                        Data = contractId,

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
        [HttpDelete("deleteContract/{contractId}")]
        public async Task<IActionResult> DeleteContractAsync(Guid contractId,Guid authorId)
        {
            try
            {
                if (await _acceptanceRecordService.HasActiveAcceptanceRecordsAsync(contractId))
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Không thể xóa do có biên bản nghiệm thu lệ thuộc."
                    });
                }
                var result = await _contractService.SoftDeleteContractAsync(contractId, authorId);
                return result ? Ok(new { message = "Xóa hợp đồng thành công." }) : NotFound(new { message = "Không tìm thấy ." });
            }
            catch (UnauthorizedAccessException ex) // Handle permission errors
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    Success = false,
                    Message = "Bạn không có quyền xóa hợp đồng.",
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
