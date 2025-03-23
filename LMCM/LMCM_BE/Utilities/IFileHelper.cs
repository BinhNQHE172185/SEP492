namespace LMCM_BE.Utilities
{
    public interface IFileHelper
    {
        Task<string> ComputeFileHashAsync(IFormFile file);
    }
}
