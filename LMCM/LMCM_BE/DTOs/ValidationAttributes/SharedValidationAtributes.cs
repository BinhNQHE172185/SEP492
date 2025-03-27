using System.ComponentModel.DataAnnotations;

namespace LMCM_BE.DTOs.Validators
{
    public class SharedValidationAtributes
    {
        public class DateMustBePresentOrFuture : ValidationAttribute
        {
            protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
            {
                if (value is DateTime date && date < DateTime.Today)
                {
                    return new ValidationResult("Ngày đề xuất không thể là ngày trong quá khứ");
                }
                return ValidationResult.Success;
            }
        }

        public class AllowedFileExtensions : ValidationAttribute
        {
            private readonly string[] _allowedExtensions;

            public AllowedFileExtensions(string[] allowedExtensions)
            {
                _allowedExtensions = allowedExtensions;
            }

            protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
            {
                if (value is IFormFile file)
                {
                    var extension = System.IO.Path.GetExtension(file.FileName);
                    if (!_allowedExtensions.Contains(extension.ToLower()))
                    {
                        return new ValidationResult($"Định dạng tệp không hợp lệ. Chỉ chấp nhận: {string.Join(", ", _allowedExtensions)}");
                    }
                }
                return ValidationResult.Success;
            }
        }

        public class MaxFileSize : ValidationAttribute
        {
            private readonly int _maxSizeInBytes;

            public MaxFileSize(int maxSizeInBytes)
            {
                _maxSizeInBytes = maxSizeInBytes;
            }

            protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
            {
                if (value is IFormFile file && file.Length > _maxSizeInBytes)
                {
                    return new ValidationResult($"Dung lượng tệp không được vượt quá {_maxSizeInBytes / (1024 * 1024)}MB");
                }
                return ValidationResult.Success;
            }
        }
    }
}
