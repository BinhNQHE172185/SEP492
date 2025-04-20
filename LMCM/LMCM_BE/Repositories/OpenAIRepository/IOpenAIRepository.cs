using static LMCM_BE.DTOs.OpenAIDtos.OpenAIDto;

namespace LMCM_BE.Repositories.OpenAIRepository
{
    public interface IOpenAIRepository
    {
        Task<string?> UploadAndAnalyzeFileAsync(Stream file, string fileName, string prompt);
    }
}
