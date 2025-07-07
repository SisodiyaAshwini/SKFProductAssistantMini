# ProductAssistantMini

An Azure Function–based application that uses Retrieval-Augmented Generation (RAG) with Azure OpenAI to answer product-related queries.

---

## 🔧 Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Azure Functions Core Tools](https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local); Check   installed version: func --version
- Azure Storage Emulator (or use `UseDevelopmentStorage=true`)
- VS Code or Visual Studio (with Azure Functions extension)
- Azure OpenAI endpoint and key

---

## How to Run the Application Locally

### 1. Clone or Download the Repository

Unzip or clone the folder on your machine.

### 2. Configure `local.settings.json`
Rename "local.settings.sample.json" file to "local.settings.json"
Replace the Azure Open AI key and Redis connection string to actual values
Place this file in the root of the `.API` project (e.g., `ProductAssistantMini.API/`):

```json
{
    "IsEncrypted": false,
    "Values": {
        "AzureWebJobsStorage": "UseDevelopmentStorage=true",
        "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated"
    },

    "AzureOpenAI": {
    "Endpoint": "https://skf-openai-dev-eval.openai.azure.com/",
    "Key": "<Key>",
    "Deployment": "gpt-4o-mini"
  },
    "SystemMessage": "You are a product assistant for a manufacturing company that produces bearings, including ball bearings and deep groove ball bearings. Use the provided information.          Don't add up the information. If the product isn’t found or the attribute doesn’t exist, handle it gracefully. Perhaps say: I’m sorry, I can’t find that information. Your           responses should be short, accurate, and specific to the product details.",
    "RedisCache":{
      "ConnectionString":"<Redis connection string>"
      }
}

3. Build and run the project
cd ProductAssistantMini.API
func start

NOTE: Make sure port 7071 is available

4. How to Test (Postman or Any API Tool)
POST Request
URL: http://localhost:7071/api/ask
Method: POST
Headers: 
Content-Type: application/json
Body (raw → JSON):
{
  "query": "What is the width of bearing 6205?"
}
