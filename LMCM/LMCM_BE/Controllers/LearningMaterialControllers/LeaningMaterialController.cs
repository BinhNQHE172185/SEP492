using LMCM_BE.DTOs.LearningMaterialDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.DTOs.UserDtos;
using LMCM_BE.Models;
using LMCM_BE.Services.ContractService;
using LMCM_BE.Services.LearningMaterialService;
using LMCM_BE.Services.SyllabusService;
using LMCM_BE.Services.UserService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LMCM_BE.Controllers.LearningMaterialControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LearningMaterialController : ControllerBase
    {
        private readonly ILearningMaterialService _learningMaterialService;
        private readonly ILearningMaterialChangesHistorySerivce _changesService;
        private readonly IUserService _userService;
        private readonly IContractService _contractService;
        private readonly ILearningMaterialDetailsService _materialDetailService;
        private readonly ISyllabusService _syllabusService;

        public LearningMaterialController(
            ILearningMaterialService learningMaterialService,
            ILearningMaterialChangesHistorySerivce changesService,
            IUserService userService,
            IContractService contractService,
            ILearningMaterialDetailsService materialDetailService,
            ISyllabusService syllabusService
            )
        {
            _learningMaterialService = learningMaterialService;
            _changesService = changesService;
            _userService = userService;
            _contractService = contractService;
            _materialDetailService = materialDetailService;
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
        [HttpPost("getLearningMaterialChangesHistoryList")]
        public async Task<IActionResult> GetLearningMaterialChangesHistoriesAsync([FromBody] PagingRequest request)
        {
            try
            {
                var data = await _changesService.GetLearningMaterialChangeHistoriesAsync(
                    request.Id, request.SearchKey, request.pageIndex, request.PageSize);

                if (data != null && data.Items.Any())
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

        [HttpPost("createChangesHistory")]
        public async Task<IActionResult> CreateLearningMaterialChangesHistory([FromBody] CreateLearningMaterialChangesHistoryDto historyDto)
        {
            if (historyDto == null)
            {
                return BadRequest("Invalid data.");
            }
            // Validate referenced IDs
            UserProfileResponseDto user = await _userService.GetProfile(historyDto.UserId.ToString());
            if (user == null)
                return BadRequest(new { message = "Invalid UserId." });

            var newMaterial = await _learningMaterialService.GetLearningMaterialByIdAsync(historyDto.NewMaterialId);
            if (newMaterial == null)
                return BadRequest(new { message = "Invalid NewMaterialId." });

            if (historyDto.OldMaterialId.HasValue)
            {
                var oldMaterial = await _learningMaterialService.GetLearningMaterialByIdAsync(historyDto.OldMaterialId.Value);
                if (oldMaterial == null)
                    return BadRequest(new { message = "Invalid OldMaterialId." });
            }

            if (historyDto.ContractId.HasValue)
            {
                var contract = await _contractService.GetContractByIdAsync(historyDto.ContractId.Value);
                if (contract == null)
                    return BadRequest(new { message = "Invalid ContractId." });
            }

            bool isSuccess = await _changesService.CreateLearningMaterialChangesHistoryAsync(historyDto);
            if (isSuccess)
                return Ok(new { message = "History created successfully" });

            return StatusCode(500, "Failed to create history.");
        }
        [HttpPost("getMaterialDetailList")]
        public async Task<IActionResult> GetMaterialDetailsListAsync([FromBody] PagingRequest request)
        {
            try
            {
                var data = await _materialDetailService.GetMaterialDetailsListAsync(request.SearchKey, request.pageIndex, request.PageSize);

                if (data != null)
                {
                    return Ok(data);
                }

                return NotFound(new { message = "Không tìm thấy dữ liệu." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi máy chủ nội bộ.", error = ex.Message });
            }
        }

        [HttpGet("detail/{id}")]
        public async Task<IActionResult> GetMaterialDetailByIdAsync(Guid id)
        {
            try
            {
                var detail = await _materialDetailService.GetMaterialDetailByIdAsync(id);
                if (detail == null)
                {
                    return NotFound(new { message = "Dữ liệu không được tìm thấy." });
                }
                return Ok(detail);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("detail/create")]
        public async Task<IActionResult> CreateMaterialDetailAsync([FromBody] LearningMaterialDetailsInsertDto detailDto)
        {
            try
            {
                var detail = await _materialDetailService.InsertMaterialDetailsAsync(detailDto);
                if (detail != null)
                {
                    return Ok(new { message = "Thêm thành công.", Data = detail });
                }
                return BadRequest(new { message = "Thêm thất bại. Vui lòng kiểm tra dữ liệu đầu vào." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("detail/update/{id}")]
        public async Task<IActionResult> UpdateMaterialDetailAsync(Guid id, [FromBody] LearningMaterialDetailsInsertDto newDetailDto)
        {
            try
            {
                bool isUpdated = await _materialDetailService.UpdateMaterialDetailAsync(id, newDetailDto);
                if (isUpdated)
                {
                    return Ok(new { message = "Cập nhật thành công." });
                }
                return NotFound(new { message = "Dữ liệu không được tìm thấy." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("detail/delete/{id}")]
        public async Task<IActionResult> DeleteMaterialDetailAsync(Guid id)
        {
            try
            {
                bool isDeleted = await _materialDetailService.DeleteMaterialDetailByIdAsync(id);
                if (isDeleted)
                {
                    return Ok(new { message = "Xóa thành công." });
                }
                return NotFound(new { message = "Dữ liệu không được tìm thấy." });
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
                var data = await _materialDetailService.GetPublishersAsync();
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
