using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Plugins.Memory;
using System.Reflection;

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("AppSettings.json", optional: true, reloadOnChange: true)
    .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
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
await textMemory.SaveInformationAsync(memoryCollectionName, id: "info5", text: "My family is from New York");

// Note: The recall function is coming from TextMemoryPlugin
// I don't really like tightly coupling plugins - I'd probably inject the facts using a handlebars template instead.

const string promptTemplate = """
                              Consider only the facts below when answering questions:

                              BEGIN FACTS
                              About me: {{recall 'where did I grow up?'}}
                              About me: {{recall 'where do I live now?'}}
                              END FACTS

                              Question: {{$input}}

                              Answer:
                              """;

var kernelFunction = kernel.CreateFunctionFromPrompt(promptTemplate, new OpenAIPromptExecutionSettings { MaxTokens = 100 });

var kernelArguments = new KernelArguments()
{
    [TextMemoryPlugin.InputParam] = "Do I live in the same town where I grew up?",
    [TextMemoryPlugin.CollectionParam] = memoryCollectionName,
    [TextMemoryPlugin.LimitParam] = "2",
    [TextMemoryPlugin.RelevanceParam] = "0.79",
};

var result = await kernel.InvokeAsync(kernelFunction, kernelArguments);

Console.WriteLine("Ask: Do I live in the same town where I grew up?");
Console.WriteLine($"Answer: {result.GetValue<string>()}");