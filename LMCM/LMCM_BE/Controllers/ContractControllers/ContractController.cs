using LMCM_BE.DTOs.ContractDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Repositories.ContractRepository;
using LMCM_BE.Services.ContractService;
using Microsoft.AspNetCore.Mvc;

namespace LMCM_BE.Controllers.ContractControllers
{
    [Route("api/contracts")]
    [ApiController]
    public class ContractController : ControllerBase
    {
        private readonly IContractService _contractService;

        public ContractController(IContractService contractService)
        {
            _contractService = contractService;
        }

        [HttpPost("getContractList")]
        public async Task<IActionResult> GetContractsAsync([FromBody] PagingRequest request)
        {
            try
            {
                var data = await _contractService.GetContractsAsync(request.SearchKey, request.pageIndex, request.PageSize);
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
        public async Task<IActionResult> CreateContract([FromForm] ContractInsertDto contractDto)
        {
            try
            {
                var contract = await _contractService.CreateContract(contractDto);
                return Ok(new
                {
                    Success = true,
                    Message = "Hợp đồng đã được tạo thành công.",
                    Data = contract
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

    }


}
