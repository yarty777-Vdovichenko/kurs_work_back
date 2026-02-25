using System.Runtime.CompilerServices;

namespace kurswork_back.Models
{
    public class Subscriber
    {
        public string? Id { get; set; }
        public string FullName { get; set; }
        public string CreatedAt { get; set; }
        public List<SimCard> Sims { get; set; }
    }
}
