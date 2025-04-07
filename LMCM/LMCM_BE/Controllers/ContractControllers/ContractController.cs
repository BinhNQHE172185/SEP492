using LMCM_BE.DTOs.BudgetProposalDtos;
using LMCM_BE.DTOs.ContractDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.DTOs.UserDtos;
using LMCM_BE.Repositories.ContractRepository;
using LMCM_BE.Services.AcceptanceRecordService;
using LMCM_BE.Services.ContractService;
using LMCM_BE.Services.UserService;
using Microsoft.AspNetCore.Mvc;

namespace LMCM_BE.Controllers.ContractControllers
{
    [Route("api/contracts")]
    [ApiController]
    public class ContractController : ControllerBase
    {
        private readonly IContractService _contractService;
        private readonly IUserService _userService;

        public ContractController(IContractService contractService, IUserService userService)
        {
            _contractService = contractService;
            _userService = userService; 
        }

        [HttpPost("getContractList")]
        public async Task<IActionResult> GetContractsAsync([FromBody] PagingRequest request)
        {
            try
            {
                UserProfileResponseDto user = await _userService.GetProfileFromCookie();
                var data = await _contractService.GetContractsAsync(user,request.SearchKey, request.pageIndex, request.PageSize);
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
        [HttpPost("getContractListNoPaging")]
        public async Task<IActionResult> GetContractsNoPagingAsync(string? searchKey)
        {
            try
            {
                UserProfileResponseDto user = await _userService.GetProfileFromCookie();
                var data = await _contractService.GetContractsAsync(user,searchKey);
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

                UserProfileResponseDto user = await _userService.GetProfileFromCookie();
                var contract = await _contractService.CreateContract(user,contractDto);
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
        public async Task<IActionResult> GetContractDetailAsync(Guid contractId)
        {
            try
            {
                UserProfileResponseDto user = await _userService.GetProfileFromCookie();
                var data = await _contractService.GetContractByIdAsync(user, contractId);
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

                UserProfileResponseDto user = await _userService.GetProfileFromCookie();
                bool isSuccess = await _contractService.UpdateContractAsync(user,id, newContract);
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
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpDelete("deleteContract/{contractId}")]
        public async Task<IActionResult> DeleteContractAsync(Guid contractId)
        {
            try
            {
                UserProfileResponseDto user = await _userService.GetProfileFromCookie();
                var result = await _contractService.SoftDeleteContractAsync(user,contractId);
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
