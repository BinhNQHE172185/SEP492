using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

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
        public class DateMustBePresentOrPast : ValidationAttribute
        {
            protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
            {
                if (value is DateTime date && date > DateTime.Today)
                {
                    return new ValidationResult("Ngày phải trước thời điểm hiện tại");
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
        public class NullableUrlAttribute : ValidationAttribute
        {
            private static readonly Regex _urlRegex = new Regex(@"^(https?|ftp):\/\/[^\s/$.?#].[^\s]*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
            {
                if (value == null) return ValidationResult.Success;
                if (value is string str)
                {
                    if (string.IsNullOrWhiteSpace(str)) return ValidationResult.Success; // Empty = OK
                    if (_urlRegex.IsMatch(str)) return ValidationResult.Success; // Valid URL
                }
                return new ValidationResult(ErrorMessage ?? "Địa chỉ URL không hợp lệ");
            }
        }
        public class IsbnAttribute : ValidationAttribute
        {
            protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
            {
                if (value is not string isbn || string.IsNullOrWhiteSpace(isbn))
                    return ValidationResult.Success; // Null or empty is valid unless required elsewhere

                isbn = isbn.Replace("-", "").Replace(" ", "").ToUpper();

                if (isbn.Length == 10)
                {
                    if (!Regex.IsMatch(isbn, @"^\d{9}[\dX]$"))
                        return new ValidationResult(ErrorMessage ?? "ISBN-10 không hợp lệ về định dạng");

                    int sum = 0;
                    for (int i = 0; i < 9; i++)
                    {
                        if (!char.IsDigit(isbn[i])) return new ValidationResult(ErrorMessage ?? "ISBN-10 chứa ký tự không hợp lệ");
                        sum += (isbn[i] - '0') * (10 - i);
                    }

                    char last = isbn[9];
                    sum += (last == 'X') ? 10 : (char.IsDigit(last) ? last - '0' : -1);

                    if (sum % 11 != 0)
                        return new ValidationResult(ErrorMessage ?? "ISBN-10 không hợp lệ (sai mã kiểm tra)");
                }
                else if (isbn.Length == 13)
                {
                    if (!Regex.IsMatch(isbn, @"^\d{13}$"))
                        return new ValidationResult(ErrorMessage ?? "ISBN-13 không hợp lệ về định dạng");

                    int sum = 0;
                    for (int i = 0; i < 13; i++)
                    {
                        int digit = isbn[i] - '0';
                        sum += (i % 2 == 0) ? digit : digit * 3;
                    }

                    if (sum % 10 != 0)
                        return new ValidationResult(ErrorMessage ?? "ISBN-13 không hợp lệ (sai mã kiểm tra)");
                }
                else
                {
                    return new ValidationResult(ErrorMessage ?? "ISBN phải có 10 hoặc 13 ký tự");
                }

                return ValidationResult.Success;
            }
        }

    }
}
