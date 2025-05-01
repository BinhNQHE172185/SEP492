using LMCM_BE.DTOs.Validators;
using LMCM_BE.Shared.Constant;
using System.ComponentModel.DataAnnotations;
using static LMCM_BE.DTOs.Validators.SharedValidationAtributes;

namespace LMCM_BE.DTOs.LearningMaterialDtos
{
    public class LearningMaterialUpdateDto
    {
        [StringLength(50, ErrorMessage = "Loại học liệu không được vượt quá 50 ký tự")]
        public string? LearningType { get; set; }

        [Range(0, 6, ErrorMessage = "Loại tài liệu phải nằm trong khoảng từ 0 đến 6")]
        public MaterialType MaterialType { get; set; }

        public bool? IsMainMaterial { get; set; }

        [StringLength(50, ErrorMessage = "Tên học liệu không được vượt quá 50 ký tự")]
        public string? MaterialName { get; set; }

        [Isbn(ErrorMessage = "ISBN không hợp lệ")]
        public string? Isbn { get; set; }

        [StringLength(100, ErrorMessage = "Tên tác giả không được vượt quá 100 ký tự")]
        public string? Author { get; set; }

        [StringLength(100, ErrorMessage = "Tên nhà xuất bản không được vượt quá 100 ký tự")]
        public string? Publisher { get; set; }

        [DateMustBePresentOrPast(ErrorMessage = "Ngày không hợp lệ")]
        public DateTime? PublishedDate { get; set; }

        [StringLength(50, ErrorMessage = "Phiên bản không được vượt quá 50 ký tự")]
        public string? Edition { get; set; }

        [NullableUrl(ErrorMessage = "Địa chỉ URL không hợp lệ")]
        public string? Url { get; set; }

        [StringLength(200, ErrorMessage = "Mục đích không được vượt quá 200 ký tự")]
        public string? Purpose { get; set; }

        [StringLength(200, ErrorMessage = "Ghi chú không được vượt quá 200 ký tự")]
        public string? Note { get; set; }
    }
}
