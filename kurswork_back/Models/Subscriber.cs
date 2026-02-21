using System.Runtime.CompilerServices;

namespace kurswork_back.Models
{
    public class Subscriber
    {
        public string? id { get; set; }
        public string fullName { get; set; }
        public string createdAt { get; set; }
        public List<SimCard> sims { get; set; }
    }
}
