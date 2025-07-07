using ProductAssistant.Data.RetrievelEngine.KeywordBasedRetriever;
using System.Text.Json;
using ProductAssistant.Services.Interface;
using ProductAssistant.Services.Models;

namespace ProductAssistant.Services.Service;

/// <summary>
/// Provides RAG functionality by combining keyword-based document search
/// with Azure OpenAI to answer user queries based on relevant product data.
/// </summary>
public class RAGService : IRAGService
{
    #region Variable decalartion
    private readonly IOpenAIService _openAIService;
    private readonly IProductKeywordRetriever _productKeywordRetriever;
    private readonly IRedisCacheService _redisCache;
    #endregion

    #region Constructor
    public RAGService(IProductKeywordRetriever productKeywordRetriever, IOpenAIService openAiService, IRedisCacheService redisCache)
    {
        _productKeywordRetriever = productKeywordRetriever;
        _openAIService = openAiService;
        _redisCache = redisCache;
    }
    #endregion

    #region Build prompt

    public string BuildPrompt(string query)
    {
        var searchedDoc = _productKeywordRetriever.Search(query);
        return $"""
            Product Data: {JsonSerializer.Serialize(searchedDoc?.Data, new JsonSerializerOptions { WriteIndented = true })}
            Question: {query}
            """;
    }
    #endregion

    #region Get answer
    public async Task<string> GetAnswerAsync(string query)
    {
        //Keywords from the query
        List<string> keywords = _productKeywordRetriever.GetKeywords(query);
        string answer;

        //Read from cache
        answer = await _redisCache.ReadfromCacheAsync(keywords);

        if (!string.IsNullOrEmpty(answer))
        {
            RedisCacheEntry? entry = JsonSerializer.Deserialize<RedisCacheEntry>(answer);
            return entry?.AIResponse ?? string.Empty;
        }
        else
        {
            string prompt = BuildPrompt(query);
            answer = await _openAIService.GetAnswerAsync(prompt);

            //Write to cache
            RedisCacheEntry cacheEntry = new RedisCacheEntry
            {
                AIResponse = answer,
                Query = query,
                Timestamp = DateTime.Now
            };
            await _redisCache.WriteToCacheAsync(keywords, cacheEntry);

            return answer;
        }
    }
    #endregion
}