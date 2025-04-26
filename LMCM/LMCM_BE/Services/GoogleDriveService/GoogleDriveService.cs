using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Upload;
using LMCM_BE.Utilities;
using System.Text.RegularExpressions;

namespace LMCM_BE.Services.GoogleDriveService
{
    public class GoogleDriveService : IGoogleDriveService
    {
        private readonly DriveService _driveService;
        private readonly IFileHelper _fileHelper;
        private readonly string _contractFolderId;
        private readonly string _budgetProposalFolderId;
        private readonly string _acceptanceRecordFolderId;
        private readonly string _documentTemplateFolderId;

        public GoogleDriveService(DriveService driveService, IFileHelper fileHelper, IConfiguration configuration)
        {
            _driveService = driveService;
            _fileHelper = fileHelper;
            _contractFolderId = configuration["GoogleDriveFolders:Contract"];
            _budgetProposalFolderId = configuration["GoogleDriveFolders:BudgetProposal"];
            _acceptanceRecordFolderId = configuration["GoogleDriveFolders:AcceptanceRecord"];
            _documentTemplateFolderId = configuration["GoogleDriveFolders:DocumentTemplate"];
        }
        private async Task<string> ExtractFileIdAsync(string url)
        {
            var match = Regex.Match(url, @"(?:/d/|id=)([a-zA-Z0-9_-]+)");
            return match.Success ? match.Groups[1].Value : null;
        }
        public async Task<string?> UploadFileAsync(IFormFile file, string folderId)
        {
            if (file == null || file.Length == 0)
                return null;

            // Step 1: Check if file already exists in the folder
            string query = $"name = '{file.FileName}' and '{folderId}' in parents and trashed = false";
            var listRequest = _driveService.Files.List();
            listRequest.Q = query;
            listRequest.Fields = "files(id, webViewLink)";
            var fileList = await listRequest.ExecuteAsync();

            var existingFile = fileList.Files.FirstOrDefault();
            if (existingFile != null)
            {
                // File already exists, return the existing link
                return existingFile.WebViewLink;
            }

            // Step 2: Upload the file
            var fileMetadata = new Google.Apis.Drive.v3.Data.File
            {
                Name = file.FileName,
                Parents = new List<string> { folderId }
            };

            using var stream = file.OpenReadStream();
            var request = _driveService.Files.Create(fileMetadata, stream, file.ContentType);
            request.Fields = "id,webViewLink";

            var result = await request.UploadAsync();
            if (result.Status != UploadStatus.Completed)
                return null;

            var uploadedFile = request.ResponseBody;
            return uploadedFile.WebViewLink;
        }
        public async Task<bool> ShareFoldersWithUserAsync(string email, bool isHod, string role)
        {
            try
            {
                var permission = new Permission
                {
                    Type = "user",
                    Role = role,  // "reader" (view-only) or "writer" (edit)
                    EmailAddress = email,
                };

                var DocumentTemplatePermissionTask = _driveService.Permissions.Create(permission, _documentTemplateFolderId).ExecuteAsync();
                if (isHod)
                {
                    var contractPermissionTask = _driveService.Permissions.Create(permission, _contractFolderId).ExecuteAsync();
                    var budgetProposalPermissionTask = _driveService.Permissions.Create(permission, _budgetProposalFolderId).ExecuteAsync();
                    var acceptanceRecordPermissionTask = _driveService.Permissions.Create(permission, _acceptanceRecordFolderId).ExecuteAsync();

                    await Task.WhenAll(DocumentTemplatePermissionTask, contractPermissionTask, budgetProposalPermissionTask, acceptanceRecordPermissionTask);
                }
                else
                {
                    await Task.WhenAll(DocumentTemplatePermissionTask);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sharing folder: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> RevokePermissionFromFolderAsync(string email, bool isHod)
        {
            try
            {
                // First, get all folder IDs to revoke permissions from
                var folderIds = new List<string> {};

                if (isHod)
                {
                    folderIds.AddRange(new List<string> { _contractFolderId, _budgetProposalFolderId, _acceptanceRecordFolderId });
                }

                // Now, revoke permissions from each folder for the provided email
                foreach (var folderId in folderIds)
                {
                    // List all permissions for the folder
                    var permissions = await _driveService.Permissions.List(folderId).ExecuteAsync();

                    // Find the permission object for the user with the provided email
                    var permissionToRevoke = permissions.Permissions?.FirstOrDefault(p => p.EmailAddress == email);

                    if (permissionToRevoke != null)
                    {
                        // Revoke the permission by deleting it
                        await _driveService.Permissions.Delete(folderId, permissionToRevoke.Id).ExecuteAsync();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error revoking permission: {ex.Message}");
                return false;
            }
        }
        public async Task<string> GetDownloadUrlAsync(string fileUrl)
        {
            string fileId = await ExtractFileIdAsync(fileUrl);

            // Generate a direct download link
            string downloadUrl = $"https://drive.usercontent.google.com/download?id={fileId}&export=download";

            return downloadUrl;
        }

        public async Task<bool> SharePdfFileWithUserAsync(string url, string email, string role = "reader")
        {
            try
            {
                string fileId = await ExtractFileIdAsync(url);
                if (fileId == null)
                    return false;

                // Check existing permissions
                var request = _driveService.Permissions.List(fileId);
                request.Fields = "permissions(id,emailAddress,type,role)";
                var permissionsList = await request.ExecuteAsync();

                Console.WriteLine("Current permissions for file:");
                foreach (var pm in permissionsList.Permissions)
                {
                    Console.WriteLine($"ID: {pm.Id}, Type: {pm.Type}, Role: {pm.Role}, Email: {pm.EmailAddress}");
                }

                var existingPermission = permissionsList.Permissions
                    .FirstOrDefault(p => p.EmailAddress != null && p.EmailAddress.Equals(email, StringComparison.OrdinalIgnoreCase));

                if (existingPermission != null)
                {
                    Console.WriteLine("User already has permission.");
                    return true; // No need to share again
                }

                // Create new permission
                var permission = new Permission
                {
                    Type = "user",
                    Role = role,
                    EmailAddress = email,
                };

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
