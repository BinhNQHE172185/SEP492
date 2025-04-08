using System.ComponentModel.DataAnnotations;

namespace LMCM_BE.DTOs.ContractValueItemDtos
{
    public class ContractValueItemDto
    {
        public Guid? ValueId { get; set; }

        [Required(ErrorMessage = "Danh mục không được để trống.")]
        public string Category { get; set; } = null!;

        [Required(ErrorMessage = "Đơn vị đo không được để trống.")]
        public string MeasurementUnit { get; set; } = null!;

        [Range(0, double.MaxValue, ErrorMessage = "Đơn giá chuẩn phải lớn hơn hoặc bằng 0.")]
        public decimal StandardRate { get; set; }

        public string? QualityRequirements { get; set; }

        [Range(0, 1, ErrorMessage = "Tỷ lệ cập nhật phải nằm trong khoảng từ 0 đến 1.")]
        public decimal ValueRatioForUpdate { get; set; } = 0.5m;

        [Range(0, double.MaxValue, ErrorMessage = "Giá trị hợp đồng phải lớn hơn hoặc bằng 0.")]
        public decimal ContractValue { get; set; }
    }

}
