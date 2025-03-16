using LMCM_BE.DTOs.LearningMaterialDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.DTOs.UserDtos;
using LMCM_BE.Models;
using LMCM_BE.Services.ContractService;
using LMCM_BE.Services.LearningMaterialService;
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

        public LearningMaterialController(
            ILearningMaterialService learningMaterialService,
            ILearningMaterialChangesHistorySerivce changesService,
            IUserService userService,
            IContractService contractService
            )
        {
            _learningMaterialService = learningMaterialService;
            _changesService = changesService;
            _userService = userService;
            _contractService = contractService;
        }

        [HttpPost("getPagedMaterialsList")]
        public async Task<IActionResult> GetMaterialsAsync([FromBody] PagingRequest request)
        {
            try
            {
                var data = await _learningMaterialService.GetMaterialsBySyllabusIdAsync((Guid)request.Id,request.SearchKey, request.pageIndex, request.PageSize);
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
                    return NotFound(new { message = "Learning material not found." });
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
                var createdMaterial = await _learningMaterialService.InsertLearningMaterialAsync(material);
                return Ok(createdMaterial);
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
                var updatedMaterial = await _learningMaterialService.UpdateLearningMaterialAsync(id, material);
                if (updatedMaterial == null)
                {
                    return NotFound(new { message = "Learning material not found." });
                }
                return Ok(updatedMaterial);
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
                    return NotFound(new { message = "Learning material not found." });
                }
                return Ok(new { message = "Learning material deleted successfully." });
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
                return NotFound(new { message = "Data not found." });
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
    }
}
