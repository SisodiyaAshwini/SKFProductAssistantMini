using System.Reflection.Metadata;
using System.Text.Json;
using ProductAssistant.Data.Models;

namespace ProductAssistant.Data.RetrievelEngine.KeywordBasedRetriever
{
    /// <summary>
    /// This class searches multiple fields for relevant keywords matching using scoring strategy
    /// </summary>
    public class ProductKeywordRetriever : IProductKeywordRetriever
    {
        #region Variable declaration
        private readonly HashSet<string> StopWords = new(StringComparer.OrdinalIgnoreCase)
            {
                "what", "is", "the", "a", "an", "of", "on", "in", "at", "to", "from", "by", "with", "about", "for", "as", "into", "like", "through"
            };
        private readonly List<ProductDocument> _documents = new();
        private readonly string _dataDirectory = Path.Combine(AppContext.BaseDirectory, "Data");
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes the ProductKeywordRetriever by loading and parsing all JSON product files
        /// from the data directory into in-memory ProductDocument objects for keyword-based search.
        /// </summary>
        public ProductKeywordRetriever()
        {
            foreach (var file in Directory.EnumerateFiles(_dataDirectory, "*.json"))
            {
                var json = File.ReadAllText(file);
                var root = JsonDocument.Parse(json).RootElement;

                var title = root.GetProperty("title").GetString() ?? "";
                var designation = root.GetProperty("designation").GetString() ?? "";

                var doc = new ProductDocument
                {
                    FilePath = file,
                    Title = title,
                    Designation = designation,
                    RawText = root.ToString(),
                    Data = JsonSerializer.Deserialize<Dictionary<string, object>>(json) ?? new()
                };

                _documents.Add(doc);
            }
        }
        #endregion

        #region Keyword based symentic search       
        /// <summary>
        /// Scoring strategy :- A match in the title is considered more important (3 points) than in the designation (2), 
        /// and much more important than a general match in raw text (1 point)
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public ProductDocument? Search(string query)
        {
            var keywords = GetKeywords(query);

            var doc = _documents
            .OrderByDescending(doc =>
            {
                int score = 0;
                foreach (var keyword in keywords)
                {
                    if (!string.IsNullOrEmpty(doc.Title) && doc.Title.ToLower().Contains(keyword)) score += 3;
                    if (!string.IsNullOrEmpty(doc.Designation) && doc.Designation.ToLower().Contains(keyword)) score += 2;
                    if (!string.IsNullOrEmpty(doc.RawText) && doc.RawText.ToLower().Contains(keyword)) score += 1;
                }
                return score;
            })
            .FirstOrDefault(doc =>
                !string.IsNullOrEmpty(doc.RawText) &&
                keywords.Any(k => doc.RawText.ToLower().Contains(k)));

            return doc;
        }

        public List<string> GetKeywords(string query)
        {
            return query
                .ToLower()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Where(word => !StopWords.Contains(word))
                .ToList();

        }
        #endregion
    }
}