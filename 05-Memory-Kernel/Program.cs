using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Plugins.Memory;

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

var azureOpenAiSettings = configuration.GetSection("AzureOpenAi");

var endpoint = azureOpenAiSettings["Endpoint"];
var apiKey = azureOpenAiSettings["ApiKey"];
var chatCompletionDeploymentName = azureOpenAiSettings["ChatCompletionDeploymentName"];
var embeddingsDeploymentName = azureOpenAiSettings["EmbeddingsDeploymentName"];

Console.Clear();

var builder = Kernel.CreateBuilder();

builder.Services
    .AddLogging(loggingBuilder => loggingBuilder.AddDebug().SetMinimumLevel(LogLevel.Trace))
    .AddAzureOpenAIChatCompletion(
        chatCompletionDeploymentName!,
        endpoint!,
        apiKey!)
    .AddAzureOpenAITextEmbeddingGeneration(
        embeddingsDeploymentName!,
        endpoint!,
        apiKey!
        );

var kernel = builder.Build();

const string memoryCollectionName = "aboutMe";

var embeddingGenerator = new AzureOpenAITextEmbeddingGenerationService(embeddingsDeploymentName!, endpoint!, apiKey!);
var memoryStore = new VolatileMemoryStore();

var textMemory = new SemanticTextMemory(memoryStore, embeddingGenerator);

var memoryPlugin = kernel.ImportPluginFromObject(new TextMemoryPlugin(textMemory));

Console.WriteLine("Available Functions:");

foreach (var function in memoryPlugin.Select(function => function.Name))
{
    Console.WriteLine(function);
}

Console.WriteLine("-----------------------");

await kernel.InvokeAsync(memoryPlugin["Save"], new KernelArguments
{
    [TextMemoryPlugin.InputParam] = "My family is from New York",
    [TextMemoryPlugin.CollectionParam] = memoryCollectionName,
    [TextMemoryPlugin.KeyParam] = "info1",
});

var result = await kernel.InvokeAsync(memoryPlugin["Retrieve"], new KernelArguments
{
    [TextMemoryPlugin.CollectionParam] = memoryCollectionName,
    [TextMemoryPlugin.KeyParam] = "info1"
});

Console.WriteLine("Get value directly:");
Console.WriteLine(result.GetValue<string>());
Console.WriteLine();

result = await kernel.InvokeAsync(memoryPlugin["Recall"], new KernelArguments
{
    [TextMemoryPlugin.InputParam] = "Ask: where do I live?",
    [TextMemoryPlugin.CollectionParam] = memoryCollectionName,
    [TextMemoryPlugin.LimitParam] = "2",
    [TextMemoryPlugin.RelevanceParam] = "0.79",
});

Console.WriteLine("Search:");
Console.WriteLine($"Answer: {result.GetValue<string>()}");
