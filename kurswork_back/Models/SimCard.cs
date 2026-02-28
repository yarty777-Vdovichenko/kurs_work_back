namespace kurswork_back.Models
{
    public class SimCard
    {
        public string? Id { get; set; }
        public string SimNumber { get; set; }
        public string Status { get; set; }
        public string CreatedAt { get; set; }
        public string TarifId { get; set; }
    }
}