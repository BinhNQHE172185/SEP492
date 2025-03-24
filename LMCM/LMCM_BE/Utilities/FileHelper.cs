using System.Security.Cryptography;
using System.Text.RegularExpressions;

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
        public async Task<string> ExtractFileIdFromUrl(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl))
                throw new ArgumentException("File URL cannot be null or empty.", nameof(fileUrl));

            var match = Regex.Match(fileUrl, @"\/d\/([^\/?]+)");
            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            throw new Exception("Invalid Google Drive URL format.");
        }

    }
}
