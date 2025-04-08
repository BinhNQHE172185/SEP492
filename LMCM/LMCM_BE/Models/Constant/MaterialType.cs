using System.ComponentModel.DataAnnotations;

namespace LMCM_BE.Models.Constant
{
    public enum MaterialType
    {
        [Display(Name = "Sách xuất bản chính thức")]
        SachXuatBanChinhThuc = 0,

        [Display(Name = "Sách xuất bản nội bộ")]
        SachXuatBanNoiBo = 1,

        [Display(Name = "e-book có bản quyền")]
        EBookCoBanQuyen = 2,

        [Display(Name = "Sách nguồn mở")]
        SachNguonMo = 3,

        [Display(Name = "Tài nguyên trên mạng")]
        TaiNguyenTrenMang = 4,

        [Display(Name = "Khóa học Udemy")]
        KhoaHocUdemy = 5,

        [Display(Name = "Học liệu")]
        HocLieu = 6
    }
}
