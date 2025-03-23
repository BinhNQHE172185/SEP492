using System.Security.Cryptography;

namespace LMCM_BE.Utilities
{
    public class FileHelper : IFileHelper
    {
        public async Task<string> ComputeFileHashAsync(IFormFile file)
        {
            using var sha256 = SHA256.Create();
            using var stream = file.OpenReadStream();
            var hashBytes = await sha256.ComputeHashAsync(stream);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}
