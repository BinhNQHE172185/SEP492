using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Services.SubjectService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static LMCM_BE.Controllers.UserControllers.UserController;

namespace LMCM_BE.Controllers.SubjectControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectController : ControllerBase
    {
        private readonly ISubjectService _subjectService;

        public SubjectController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        [HttpPost("getSubjectList")]
        public async Task<IActionResult> GetSubjectAsync([FromBody] PagingRequest request)
        {
            try
            {
                var data = await _subjectService.GetSubjectsAsync(request.SearchKey, request.pageIndex, request.PageSize);
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
    }
}
