using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Upload;
using LMCM_BE.DTOs.UserDtos;
using LMCM_BE.Services.UserService;
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
        private readonly string _hodEmail;

        public GoogleDriveService(DriveService driveService, IFileHelper fileHelper, IConfiguration configuration)
        {
            _driveService = driveService;
            _fileHelper = fileHelper;
            _contractFolderId = configuration["GoogleDriveFolders:Contract"];
            _budgetProposalFolderId = configuration["GoogleDriveFolders:BudgetProposal"];
            _acceptanceRecordFolderId = configuration["GoogleDriveFolders:AcceptanceRecord"];
            _documentTemplateFolderId = configuration["GoogleDriveFolders:DocumentTemplate"];
            _hodEmail= configuration["HeadOfDepartment:Email"];
        }
        private async Task<string> ExtractFileIdAsync(string url)
        {
            var match = Regex.Match(url, @"(?:/d/|id=)([a-zA-Z0-9_-]+)");
            return match.Success ? match.Groups[1].Value : null;
        }
        private async Task<string?> CreateFolderAsync(string folderName, string parentFolderId)
        {
            try
            {
                var fileMetadata = new Google.Apis.Drive.v3.Data.File
                {
                    Name = folderName,
                    MimeType = "application/vnd.google-apps.folder",
                    Parents = new List<string> { parentFolderId }
                };

                var request = _driveService.Files.Create(fileMetadata);
                request.Fields = "id,webViewLink";

                var folder = await request.ExecuteAsync();

                Console.WriteLine($"Created folder '{folderName}' with ID: {folder.Id}");

                return folder.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating folder: {ex.Message}");
                return null;
            }
        }
        private async Task ShareFolderWithUserAsync(string folderId, string email, string role)
        {
            var permission = new Permission
            {
                Type = "user",
                Role = role, // "reader" or "writer"
                EmailAddress = email
            };

            await _driveService.Permissions.Create(permission, folderId).ExecuteAsync();
        }

        public async Task<Dictionary<string, string>> CreateDefaultFoldersAsync(string rootFolderName = "LMCM")
        {
            var createdFolders = new Dictionary<string, string>();

            try
            {
                // Step 1: Create the root folder
                var rootId = await CreateFolderAsync(rootFolderName, null);
                if (rootId == null)
                    throw new Exception("Failed to create root folder.");

                // Step 2: Create subfolders under root
                var subfolders = new List<string>
                {
                    "Contract",
                    "BudgetProposal",
                    "AcceptanceRecord",
                    "DocumentTemplate"
                };

                foreach (var name in subfolders)
                {
                    var folderId = await CreateFolderAsync(name, rootId);
                    if (folderId != null)
                    {
                        createdFolders[name] = folderId;
                    }
                    await ShareFolderWithUserAsync(folderId, _hodEmail,"writer");
                }

                Console.WriteLine("Folder structure created:");
                foreach (var kvp in createdFolders)
                {
                    Console.WriteLine($"{kvp.Key}: {kvp.Value}");
                }

                return createdFolders;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating folder structure: {ex.Message}");
                return createdFolders;
            }
        }
        public async Task<List<string>> ListFolderPermissionsAsync(string folderId)
        {
            var emails = new List<string>();

            try
            {
                var request = _driveService.Permissions.List(folderId);
                request.Fields = "permissions(id,emailAddress,role,type)";
                var response = await request.ExecuteAsync();

                if (response.Permissions != null)
                {
                    foreach (var permission in response.Permissions)
                    {
                        if (permission.Type == "user" && !string.IsNullOrEmpty(permission.EmailAddress))
                        {
                            emails.Add($"{permission.EmailAddress} ({permission.Role})");
                        }
                    }
                }

                return emails;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error listing permissions: {ex.Message}");
                return emails;
            }
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
        public async Task<string?> UploadFileAsync(IFormFile file,string folderId)
        {
            if (file == null || file.Length == 0)
                return null;

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
            return uploadedFile.WebViewLink; // Return Google Drive File URL
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
                var permission = new Permission
                {
                    Type = "user",
                    Role = role, // "reader" (view-only) or "writer" (edit)
                    EmailAddress = email,
                };

                string fileId = await ExtractFileIdAsync(url);

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
