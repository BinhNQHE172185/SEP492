using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace LMCM_BE.Services.GoogleDriveService
{
    public class GoogleDriveService
    {
        private readonly DriveService _driveService;
        private readonly string _contractFolderId = "1D-BiSw2okv50bU3C85NS4Z1YE4it0374";
        private readonly string _budgetPropasalFolderId = "1-Wl5_HyRdbG9j3vBI5ynSHr7vykF7P3S";

        public GoogleDriveService()
        {
            var credential = GoogleCredential.FromFile("google-drive-credentials.json")
                .CreateScoped(DriveService.ScopeConstants.DriveFile);

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
    }
}
