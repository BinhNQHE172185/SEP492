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

        [HttpPost("getSubjectList")] //Demo
        public async Task<IActionResult> GetSubjectAsync([FromBody] string id)
        {
            try
            {
                var data = await _subjectService.GetSubjectAsync(id);
                if (data != null)
                {
                    return Ok(data);
                }
                return NotFound(new { message = "User not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
