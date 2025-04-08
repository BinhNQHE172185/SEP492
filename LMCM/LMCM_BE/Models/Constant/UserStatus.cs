using System.ComponentModel.DataAnnotations;

namespace LMCM_BE.Models.Constant
{
    public enum UserStatus
    {
        [Display(Name = "Đang chờ")]
        Pending = 1,

        [Display(Name = "Hoạt động")]
        Active = 2,

        [Display(Name = "Ngừng hoạt động")]
        Stopped = 3
    }
}
