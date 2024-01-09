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

Console.WriteLine("Available Plugin Functions:");

foreach (var function in memoryPlugin.Select(function => function.Name))
{
    Console.WriteLine(function);
}

Console.WriteLine("-----------------------");

await textMemory.SaveInformationAsync(memoryCollectionName, id: "info1", text: "My name is Andrea");
await textMemory.SaveInformationAsync(memoryCollectionName, id: "info2", text: "I work as a tourist operator");
await textMemory.SaveInformationAsync(memoryCollectionName, id: "info3", text: "I've been living in Seattle since 2005");
await textMemory.SaveInformationAsync(memoryCollectionName, id: "info4", text: "I visited France and Italy five times since 2015");

await kernel.InvokeAsync(memoryPlugin["Save"], new KernelArguments
{
    [TextMemoryPlugin.InputParam] = "My family is from New York",
    [TextMemoryPlugin.CollectionParam] = memoryCollectionName,
    [TextMemoryPlugin.KeyParam] = "info5",
});

var result = await kernel.InvokeAsync(memoryPlugin["Retrieve"], new KernelArguments
{
    [TextMemoryPlugin.CollectionParam] = memoryCollectionName,
    [TextMemoryPlugin.KeyParam] = "info1"
});

Console.WriteLine("Get a memory by ID:info1");
Console.WriteLine(result.GetValue<string>());
Console.WriteLine("-------------------");

result = await kernel.InvokeAsync(memoryPlugin["Recall"], new KernelArguments
{
    [TextMemoryPlugin.InputParam] = "Ask: where do I live?",
    [TextMemoryPlugin.CollectionParam] = memoryCollectionName,
    [TextMemoryPlugin.LimitParam] = "2",
    [TextMemoryPlugin.RelevanceParam] = "0.79",
});

Console.WriteLine("Search for a memory:");
Console.WriteLine($"Answer: {result.GetValue<string>()}");
