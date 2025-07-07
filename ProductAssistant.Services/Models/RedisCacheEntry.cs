using System.Dynamic;

namespace ProductAssistant.Services.Models
{
    public class RedisCacheEntry
    {
        public string? Query { get; set; }
       // public List<string>? Keywords { get; set; }
        public string? AIResponse { get; set; }
        public DateTime Timestamp{ get; set; }
    }
}