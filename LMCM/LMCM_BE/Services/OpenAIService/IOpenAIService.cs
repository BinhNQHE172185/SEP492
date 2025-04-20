using static LMCM_BE.DTOs.OpenAIDtos.OpenAIDto;
namespace LMCM_BE.Services.OpenAIService
{
    public interface IOpenAIService
    {
        Task<ContractInfoFromAIDto?> UploadAndAnalyzeContractFileAsync(Stream fileStream, string fileName, string prompt);
        Task<AcceptantRecordInfoFromAIDto?> UploadAndAnalyzeRecordFileAsync(Stream fileStream, string fileName, string prompt);
    }
}
