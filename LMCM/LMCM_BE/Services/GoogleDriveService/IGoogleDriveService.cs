namespace LMCM_BE.Services.GoogleDriveService
{
    public interface IGoogleDriveService
    {
        Task<string?> UploadContractFileAsync(IFormFile file);
        Task<string?> UploadBudgetPropasalFileAsync(IFormFile file);
        Task<string?> UploadAcceptanceRecordFileAsync(IFormFile file);
        Task<bool> ShareFoldersWithUser(string email, string role = "reader");
    }
}
