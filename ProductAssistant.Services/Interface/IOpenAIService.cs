namespace ProductAssistant.Services.Interface;

public interface IOpenAIService
{
    Task<string> GetAnswerAsync(string prompt);
}