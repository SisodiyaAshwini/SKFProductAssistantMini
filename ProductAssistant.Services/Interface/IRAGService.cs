namespace ProductAssistant.Services.Interface;

/// <summary>
/// Defines the contract for RAG (Retrieval-Augmented Generation) orchestration.
/// This service aggregates retrieved content with the user query to construct a prompt,
/// and sends it to the language model for response generation.
/// </summary>
public interface IRAGService
{
    /// <summary>
    /// Executes the full RAG process: retrieves relevant context and generates a response.
    /// </summary>
    /// <param name="query">The user's input query.</param>
    /// <returns>Generated answer from the language model.</returns>
    Task<string> GetAnswerAsync(string query);
}