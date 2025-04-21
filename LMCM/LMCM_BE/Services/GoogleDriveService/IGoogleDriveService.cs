namespace LMCM_BE.Services.GoogleDriveService
{
    public interface IGoogleDriveService
    {
        Task<string> ComputeGoogleDriveFileHashAsync(string fileUrl);
        //Task<(byte[]? FileContent, string? FileName)> FetchFileAsync(string fileId);
        Task<string?> UploadContractFileAsync(IFormFile file);
        Task<string?> UploadBudgetProposalFileAsync(IFormFile file);
        Task<string?> UploadAcceptanceRecordFileAsync(IFormFile file);
        Task<string?> UploadDocumentTemplateFileAsync(IFormFile file);
        Task<bool> ShareFoldersWithUserAsync(string email, string role = "reader");
        Task<bool> ShareFoldersWithHeadOfDepartmentAsync(string email, string role = "reader");
        Task<bool> SharePdfFileWithUserAsync(string url, string email, string role = "reader");
        Task<string> GetDownloadUrlAsync(string fileUrl);
    }
}
