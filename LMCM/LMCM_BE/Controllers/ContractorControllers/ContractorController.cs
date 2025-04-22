using LMCM_BE.DTOs.ContractorDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Services.ContractorService;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.ComponentModel.DataAnnotations;

namespace LMCM_BE.Controllers.ContractorControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractorController : ControllerBase
    {
        private readonly IContractorService _contractorService;

        public ContractorController(IContractorService contractorService)
        {
            _contractorService = contractorService;
        }

        [HttpPost("getContractorList")]
        public async Task<IActionResult> GetContractorsAsync([FromBody] PagingRequest request)
        {
            try
            {
                var data = await _contractorService.GetContractorsAsync(request.SearchKey, request.pageIndex, request.PageSize);
                return data != null ? Ok(data) : NotFound(new { message = "Không tìm thấy dữ liệu chuyên gia." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpGet("getAllContractorList")]
        public async Task<IActionResult> GetContractorsListAsync(string? searchKey)
        {
            try
            {
                var data = await _contractorService.GetContractorsListAsync(searchKey);
                return data != null ? Ok(data) : NotFound(new { message = "Không tìm thấy dữ liệu chuyên gia." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateContractor([FromBody] ContractorCreateDto request)
        {
            try
            {
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

                var result = await _contractorService.CreateContractorAsync(request);
                return result == true ? Ok(new { message = "Tạo chuyên gia thành công." }) : BadRequest(new { message = "Không thể tạo chuyên gia." });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = ex.Message,
                    Error = ex.Message
                });
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

        [HttpPut("update/{contractorId}")]
        public async Task<IActionResult> UpdateContractor(Guid contractorId, [FromBody] ContractorUpdateDto request)
        {
            try
            {
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
                var result = await _contractorService.UpdateContractorAsync(contractorId, request);
                return result != null ? Ok(new { message = "Cập nhật chuyên gia thành công.", contractorId = result }) : NotFound(new { message = "Không tìm thấy chuyên gia hoặc cập nhật thất bại." });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = ex.Message,
                    Error = ex.Message
                });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpDelete("{contractorId}")]
        public async Task<IActionResult> DeleteContractor(Guid contractorId)
        {
            try
            {
                var result = await _contractorService.SoftDeleteContractorAsync(contractorId);
                return result ? Ok(new { message = "Xóa chuyên gia thành công." }) : NotFound(new { message = "Không tìm thấy chuyên gia hoặc đã bị xóa trước đó." });
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

        [HttpGet("{contractorId}")]
        public async Task<IActionResult> GetContractorDetail(Guid contractorId)
        {
            try
            {
                var contractor = await _contractorService.GetContractorDetailAsync(contractorId);
                return Ok(contractor);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }
    }
}
