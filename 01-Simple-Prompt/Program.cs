using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

var azureOpenAiSettings = configuration.GetSection("AzureOpenAi");

var endpoint = azureOpenAiSettings["Endpoint"];
var apiKey = azureOpenAiSettings["ApiKey"];
var chatCompletionDeploymentName = azureOpenAiSettings["ChatCompletionDeploymentName"];
var chatCompletionModelId = azureOpenAiSettings["ChatCompletionModelId"];

Console.Clear();

var builder = Kernel.CreateBuilder();

builder.Services
    .AddLogging(loggingBuilder => loggingBuilder.AddConsole().SetMinimumLevel(LogLevel.Information))
    .AddAzureOpenAIChatCompletion(
        chatCompletionDeploymentName!,
        endpoint!,
        apiKey!,
        modelId: chatCompletionModelId);

var kernel = builder.Build();

Console.WriteLine(await kernel.InvokePromptAsync("Create a three line poem about a dog"));

Console.WriteLine(await kernel.InvokePromptAsync("What is the intent of this request? Plan a holiday to greece"));