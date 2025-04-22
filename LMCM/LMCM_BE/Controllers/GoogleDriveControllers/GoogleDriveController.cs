using Microsoft.AspNetCore.Mvc;
using LMCM_BE.Services.GoogleDriveService;

namespace LMCM_BE.Controllers.GoogleDriveControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GoogleDriveController : ControllerBase
    {
        private readonly IGoogleDriveService _googleDriveService;
        private readonly IWebHostEnvironment _env;

        public GoogleDriveController(IGoogleDriveService googleDriveService, IWebHostEnvironment env)
        {
            _googleDriveService = googleDriveService;
            _env = env;
        }

        // ⚠️ Dev-only endpoint
        [HttpPost("create-default-folders")]
        public async Task<IActionResult> CreateDefaultFolders()
        {
            if (!_env.IsDevelopment())
                return Forbid("This endpoint is only available in development mode.");

            var folderIds = await _googleDriveService.CreateDefaultFoldersAsync();

            return Ok(folderIds);
        }
        [HttpGet("permissions/{folderId}")]
        public async Task<IActionResult> GetFolderPermissions(string folderId)
        {
            var result = await _googleDriveService.ListFolderPermissionsAsync(folderId);

            if (result == null || result.Count == 0)
            {
                return NotFound("No permissions found or folder does not exist.");
            }

            return Ok(result);
        }
    }
}
