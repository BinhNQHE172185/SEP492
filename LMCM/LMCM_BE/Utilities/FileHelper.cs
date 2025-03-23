using System.Security.Cryptography;

namespace LMCM_BE.Utilities
{
    public class FileHelper : IFileHelper
    {
        public async Task<string> ComputeFileHashAsync(IFormFile file)
        {
            using var md5 = MD5.Create();
            using var stream = file.OpenReadStream();
            var hashBytes = await md5.ComputeHashAsync(stream);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}
