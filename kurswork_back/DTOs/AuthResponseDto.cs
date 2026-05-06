namespace kurswork_back.DTOs
{
    public class AuthResponseDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string Role { get; set; }
        public UserDto User { get; set; }
    }
}
