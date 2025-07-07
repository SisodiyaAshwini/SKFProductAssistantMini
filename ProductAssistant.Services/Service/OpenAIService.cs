using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;
using ProductAssistant.Services.Interface;

namespace ProductAssistant.Services.Service
{
    public class OpenAIService : IOpenAIService
    {

        #region Variable declaration
        private readonly AzureOpenAIClient _client;
        private readonly string _deploymentName;

        private readonly IConfiguration _config;
        //public ChatCompletionOptions Options { get; set; }

        #endregion

        #region Constructor
        /// <summary>
        /// Read Configurations
        /// </summary>
        /// <param name="config"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public OpenAIService(IConfiguration config)
        {
            _config = config;
            var openAISection = _config.GetSection("AzureOpenAI");

            var endpoint = openAISection.GetValue<string>("Endpoint") ?? throw new ArgumentNullException("AzureOpenAI:Endpoint");
            var key = openAISection.GetValue<string>("Key") ?? throw new ArgumentNullException("AzureOpenAI:Key");
            _deploymentName = openAISection.GetValue<string>("Deployment") ?? throw new ArgumentNullException("AzureOpenAI:Deployment");

            _client = new(new Uri(endpoint), new AzureKeyCredential(key));
        }
        #endregion

        #region Get answer from Azure OpenAI
        /// <summary>
        /// Sends a user prompt and system message to the Azure OpenAI chat model using streaming API.
        /// Reads the system prompt from configuration, builds the chat message sequence,
        /// and asynchronously accumulates the streamed response content into a single message string.
        /// Returns the full model response, or a fallback message if the response is empty.
        /// </summary>
        public async Task<string> GetAnswerAsync(string prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt))
                throw new ArgumentException("Prompt cannot be null or empty", nameof(prompt));

            ChatClient chatClient = _client.GetChatClient(_deploymentName);

            string? systemMessage = _config.GetValue<string>("SystemMessage");

            var Options = new ChatCompletionOptions
            {
                Temperature = 0.0f,         //Near-zero hallucination
                TopP = 1.0f,
                MaxOutputTokenCount = 300
            };

            ChatCompletion completion = chatClient.CompleteChat(
            [

                new SystemChatMessage(systemMessage),
                new UserChatMessage(prompt)
            ],
            Options);

            return string.IsNullOrWhiteSpace(completion.Content[0].Text) ? "[No response from model]" : completion.Content[0].Text;
        }

        #endregion
    }
}
