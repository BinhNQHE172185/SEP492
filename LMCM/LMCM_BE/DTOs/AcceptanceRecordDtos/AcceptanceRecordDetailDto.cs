using LMCM_BE.DTOs.ContractDtos;
using LMCM_BE.DTOs.UserDtos;
using LMCM_BE.Shared.Constant;

namespace LMCM_BE.DTOs.AcceptanceRecordDtos
{
    public class AcceptanceRecordDetailDto
    {
        public Guid AcceptanceId { get; set; }

        public Guid AuthorId { get; set; }

        public Guid ContractId { get; set; }

        public string Title { get; set; } = null!;

        public DateTime? AcceptanceDate { get; set; }

        public decimal? FinalPrice { get; set; }

        public string? Url { get; set; }

        public GenericStatus Status { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public string? DownloadUrl { get; set; }


        public virtual ListUserResponseDto Author { get; set; } = null!;

        public virtual ContractListDto Contract { get; set; } = null!;
        //public byte[]? FileContent { get; set; }
        //public string? FileName { get; set; }
    }
}
