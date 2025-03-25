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
        public string? Status { get; set; }
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
}
