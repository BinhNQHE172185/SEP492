namespace LMCM_BE.Services.GoogleDriveService
{
    public interface IGoogleDriveService
    {
        Task<Dictionary<string, string>> CreateDefaultFoldersAsync(string rootFolderName = "LMCM");
        Task<List<string>> ListFolderPermissionsAsync(string folderId);
        Task<string> ComputeGoogleDriveFileHashAsync(string fileUrl);
        Task<string?> UploadFileAsync(IFormFile file,string folderId);
        Task<bool> ShareFoldersWithUserAsync(string email, bool isHod, string role);
        Task<bool> RevokePermissionFromFolderAsync(string email, bool isHod);
        Task<bool> SharePdfFileWithUserAsync(string url, string email, string role);
        Task<string> GetDownloadUrlAsync(string fileUrl);
    }
}
