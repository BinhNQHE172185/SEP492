using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Upload;
using LMCM_BE.Services.UserService;
using LMCM_BE.Utilities;
using System.Text.RegularExpressions;

namespace LMCM_BE.Services.GoogleDriveService
{
    public class GoogleDriveService : IGoogleDriveService
    {
        private readonly DriveService _driveService;
        private readonly IFileHelper _fileHelper;
        private readonly string _contractFolderId = "1D-BiSw2okv50bU3C85NS4Z1YE4it0374";
        private readonly string _budgetPropasalFolderId = "1-Wl5_HyRdbG9j3vBI5ynSHr7vykF7P3S";
        private readonly string _acceptanceRecordFolderId = "1065sqU0hsl1EwZuiibSLya5DFmQMZ6s2";

        public GoogleDriveService(DriveService driveService, IFileHelper fileHelper)
        {
            _driveService = driveService;
            _fileHelper = fileHelper;
        }

        public async Task<string> ComputeGoogleDriveFileHashAsync(string fileUrl)
        {
            string fileId = await _fileHelper.ExtractFileIdFromUrl(fileUrl);

            var request = _driveService.Files.Get(fileId);
            request.Fields = "md5Checksum";  // Request only the MD5 checksum

            var file = await request.ExecuteAsync();

            if (file.Md5Checksum == null)
                throw new Exception("MD5 checksum not available for this file.");

            return file.Md5Checksum.ToLower();
        }
        public async Task<string?> UploadAcceptanceRecordFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            var fileMetadata = new Google.Apis.Drive.v3.Data.File
            {
                Name = file.FileName,
                Parents = new List<string> { _acceptanceRecordFolderId }
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

        public async Task<(byte[]? FileContent, string? FileName)> FetchFileAsync(string fileId)
        {
            if (string.IsNullOrEmpty(fileId))
                return (null, null);

            try
            {
                // Retrieve file metadata (including name)
                var request = _driveService.Files.Get(fileId);
                request.Fields = "name"; // Only fetch the file name
                var fileMetadata = await request.ExecuteAsync();
                string fileName = fileMetadata?.Name ?? "UnknownFile.pdf";

                // Download file content
                using var stream = new MemoryStream();
                await _driveService.Files.Get(fileId).DownloadAsync(stream);

                return (stream.ToArray(), fileName);
            }
            catch (Google.GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                Console.WriteLine($"Error: Unauthorized access to file {fileId}");
                return (null, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching file: {ex.Message}");
                return (null, null);
            }
        }


        public async Task<string?> UploadBudgetProposalFileAsync(IFormFile file)
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

                var contractPermissionTask = _driveService.Permissions.Create(permission, _contractFolderId).ExecuteAsync();
                var budgetProposalPermissionTask = _driveService.Permissions.Create(permission, _budgetPropasalFolderId).ExecuteAsync();
                var acceptanceRecordPermissionTask = _driveService.Permissions.Create(permission, _acceptanceRecordFolderId).ExecuteAsync();

                await Task.WhenAll(contractPermissionTask, budgetProposalPermissionTask, acceptanceRecordPermissionTask);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sharing folder: {ex.Message}");
                return false;
            }
        }
        private async Task<string> ExtractFileId(string url)
        {
            var match = Regex.Match(url, @"(?:/d/|id=)([a-zA-Z0-9_-]+)");
            return match.Success ? match.Groups[1].Value : null;
        }

        public async Task<bool> SharePdfFileWithUser(string url, string email, string role = "reader")
        {
            try
            {
                var permission = new Permission
                {
                    Type = "user",
                    Role = role, // "reader" (view-only) or "writer" (edit)
                    EmailAddress = email,
                };

                string fileId = await ExtractFileId(url);

                Console.WriteLine(fileId);
                // Share the specific PDF file
                if (fileId != null)
                    await _driveService.Permissions.Create(permission, fileId).ExecuteAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sharing PDF file: {ex.Message}");
                return false;
            }
        }

    }
}
