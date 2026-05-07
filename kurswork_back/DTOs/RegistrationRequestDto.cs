namespace kurswork_back.DTOs
{
    public class RegistrationRequestDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
