namespace kurswork_back.Models
{
    public class SimCard
    {
        public string? id { get; set; }
        public string simNumber { get; set; }
        public string status { get; set; }
        public string createdAt { get; set; }
        public Tarif tarif { get; set; }
    }
}