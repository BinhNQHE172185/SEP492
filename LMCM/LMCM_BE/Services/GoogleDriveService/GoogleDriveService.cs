using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Upload;

namespace LMCM_BE.Services.GoogleDriveService
{
    public class GoogleDriveService: IGoogleDriveService
    {
        private readonly DriveService _driveService;
        private readonly string _contractFolderId = "1D-BiSw2okv50bU3C85NS4Z1YE4it0374";
        private readonly string _budgetPropasalFolderId = "1-Wl5_HyRdbG9j3vBI5ynSHr7vykF7P3S";

        public GoogleDriveService()
        {
            GoogleCredential credential;

            //  Load credentials only ONCE
            using (var stream = new FileStream("google-drive-credentials.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(new[] { DriveService.Scope.Drive }); //  Full Drive access
            }

            _driveService = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "LMCM"
            });
        }

        public async Task<string?> UploadContractFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            var fileMetadata = new Google.Apis.Drive.v3.Data.File
            {
                Name = file.FileName,
                Parents = new List<string> { _contractFolderId }
            };

            using var stream = file.OpenReadStream();
            var request = _driveService.Files.Create(fileMetadata, stream, file.ContentType);
            request.Fields = "id,webViewLink";

            var result = await request.UploadAsync();
            if (result.Status != UploadStatus.Completed)
                return null;

            var uploadedFile = request.ResponseBody;
            return uploadedFile.WebViewLink; // Return Google Drive File URL
        }
        public async Task<string?> UploadBudgetPropasalFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            var fileMetadata = new Google.Apis.Drive.v3.Data.File
            {
                Name = file.FileName,
                Parents = new List<string> { _budgetPropasalFolderId }
            };

            using var stream = file.OpenReadStream();
            var request = _driveService.Files.Create(fileMetadata, stream, file.ContentType);
            request.Fields = "id,webViewLink";

            var result = await request.UploadAsync();
            if (result.Status != UploadStatus.Completed)
                return null;

            var uploadedFile = request.ResponseBody;
            return uploadedFile.WebViewLink; // Return Google Drive File URL
        }
        public async Task<bool> ShareFoldersWithUser(string email, string role = "reader")
        {
            try
            {
                var permission = new Permission
                {
                    Type = "user",
                    Role = role,  // "reader" (view-only) or "writer" (edit)
                    EmailAddress = email,
                };

                await _driveService.Permissions.Create(permission, _contractFolderId).ExecuteAsync();
                await _driveService.Permissions.Create(permission, _budgetPropasalFolderId).ExecuteAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sharing folder: {ex.Message}");
                return false;
            }
        }
    }
}
