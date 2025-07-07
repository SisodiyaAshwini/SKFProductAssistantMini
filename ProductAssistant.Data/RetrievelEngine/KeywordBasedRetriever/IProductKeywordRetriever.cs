using ProductAssistant.Data.Models;

namespace ProductAssistant.Data.RetrievelEngine.KeywordBasedRetriever
{
    public interface IProductKeywordRetriever
    {
        ProductDocument? Search(string query);

        List<string> GetKeywords(string query);
    }
}