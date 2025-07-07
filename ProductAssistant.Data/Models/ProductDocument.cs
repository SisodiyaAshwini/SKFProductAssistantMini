namespace ProductAssistant.Data.Models
{
    public class ProductDocument
    {
        public string FilePath { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Designation { get; set; } = default!;
        public string RawText { get; set; } = default!;
        public Dictionary<string, object> Data { get; set; } = new();
    }
}