using static LMCM_BE.DTOs.OpenAIDtos.OpenAIDto;
namespace LMCM_BE.Services.OpenAIService
{
    public interface IOpenAIService
    {
        Task<ContractorInfoFromAIDto?> UploadAndAnalyzeFileAsync(Stream fileStream, string fileName, string prompt);
    }
}
