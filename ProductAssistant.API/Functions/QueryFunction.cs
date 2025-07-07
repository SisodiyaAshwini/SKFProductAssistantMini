using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using ProductAssistant.Services.Interface;
using ProductAssistant.Models;
using System.Text.Json;
using Microsoft.Extensions.Logging;


namespace ProductAssistantMini.API.Functions
{
    public class QueryFunction
    {
        #region Variable Declarations
        private readonly IRAGService _ragService;
        private readonly ILogger<QueryFunction> _logger;

        #endregion

        #region Constructor
        public QueryFunction(IRAGService ragService, ILogger<QueryFunction> logger)
        {
            _ragService = ragService;
            _logger = logger;
        }
        #endregion

        #region Azure function HTTP trigger for product assistance
        [Function("Query")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "ask")] HttpRequestData req)
        {
            try
            {
                string body = await new StreamReader(req.Body).ReadToEndAsync();
                QueryRequest? input = JsonSerializer.Deserialize<QueryRequest>(body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (input == null || string.IsNullOrWhiteSpace(input.Query))
                {
                    _logger.LogWarning("Invalid request: Query is null or empty.");
                    var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                    await badResponse.WriteStringAsync("Invalid request. 'query' field is required.");
                    return badResponse;
                }

                string answer = await _ragService.GetAnswerAsync(input?.Query ?? "");

                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(new QueryResponse { Answer = answer });

                _logger.LogInformation("Response generated successfully.");
                return response;
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "JSON deserialization error.");
                var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await errorResponse.WriteStringAsync("Invalid JSON format.");
                return errorResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred.");
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync("An unexpected error occurred.");
                return errorResponse;
            }
        }
        #endregion
    }
}