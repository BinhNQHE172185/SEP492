using LMCM_BE.Shared.Constant;

namespace LMCM_BE.DTOs.UserDtos
{
    public class UserProfileResponseDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public List<string> Roles { get; set; }
        public string? Email { get; set; }
        public string? Picture { get; set; }
    }

    public class ListUserResponseDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public UserStatus Status { get; set; }
        public List<string> Roles { get; set; }
    }

    public class UserLoginResponseDto
    {
        public Guid? Id { get; set; }
        public string? Token { get; set; }
    }

    public class GoogleLoginRequest
    {
        public string Token { get; set; }
    }

    public class StaffRequest
    {
        public string StaffId { get; set; }
    }
    public class AssignRole
    {
        public string userId { get; set; }
        public string newRole { get; set; }
    }
    public class UpdateStatus
    {
        public string userId { get; set; }
        public string status { get; set; }
    }
    public class UpdateUser
    {
        public string userId { get; set; }
        public string staffId { get; set; }
    }
}
