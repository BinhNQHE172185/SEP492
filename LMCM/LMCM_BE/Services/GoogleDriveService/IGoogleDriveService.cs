namespace LMCM_BE.Services.GoogleDriveService
{
    public interface IGoogleDriveService
    {
        Task<string> ComputeGoogleDriveFileHashAsync(string fileUrl);
        Task<(byte[]? FileContent, string? FileName)> FetchFileAsync(string fileId);
        Task<string?> UploadContractFileAsync(IFormFile file);
        Task<string?> UploadBudgetProposalFileAsync(IFormFile file);
        Task<string?> UploadAcceptanceRecordFileAsync(IFormFile file);
        Task<bool> ShareFoldersWithUser(string email, string role = "reader");
    }
}
