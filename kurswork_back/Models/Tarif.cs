namespace kurswork_back.Models
{
    public class Tarif
    {
        public string? id { get; set; }
        public string name { get; set; }
        public double internet_capacity { get; set; }
        public int minutes { get; set; }
        public string additional { get; set; }
        public double price { get; set; }
    }
}
