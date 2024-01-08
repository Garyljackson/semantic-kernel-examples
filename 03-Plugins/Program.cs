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
    .AddLogging(loggingBuilder => loggingBuilder.AddDebug().SetMinimumLevel(LogLevel.Trace))
    .AddAzureOpenAIChatCompletion(
        chatCompletionDeploymentName!,
        endpoint!,
        apiKey!,
        modelId: chatCompletionModelId);

var kernel = builder.Build();

var pluginFolder = Path.Combine(Directory.GetCurrentDirectory(), "Plugins");

var prompts = kernel.CreatePluginFromPromptDirectory(pluginFolder);

var kernelArguments = new KernelArguments { { "input", "The stars are so bright tonight" } };

var result = await kernel.InvokeAsync(prompts["Sherlock"], kernelArguments);

Console.WriteLine(result);