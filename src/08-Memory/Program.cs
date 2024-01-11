using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Memory;
using System.Reflection;

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("AppSettings.json", optional: true, reloadOnChange: true)
    .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
    .Build();

var azureOpenAiSettings = configuration.GetSection("AzureOpenAi");

var endpoint = azureOpenAiSettings["Endpoint"];
var apiKey = azureOpenAiSettings["ApiKey"];
var embeddingsDeploymentName = azureOpenAiSettings["EmbeddingsDeploymentName"];

Console.Clear();

const string memoryCollectionName = "aboutMe";

var embeddingGenerator = new AzureOpenAITextEmbeddingGenerationService(embeddingsDeploymentName!, endpoint!, apiKey!);
var memoryStore = new VolatileMemoryStore();

var textMemory = new SemanticTextMemory(memoryStore, embeddingGenerator);

await textMemory.SaveInformationAsync(memoryCollectionName, id: "info1", text: "My name is Andrea");
await textMemory.SaveInformationAsync(memoryCollectionName, id: "info2", text: "I work as a tourist operator");
await textMemory.SaveInformationAsync(memoryCollectionName, id: "info3", text: "I've been living in Seattle since 2005");
await textMemory.SaveInformationAsync(memoryCollectionName, id: "info4", text: "I visited France and Italy five times since 2015");

// Get a specific memory by ID
var getMemoryResult = await textMemory.GetAsync(memoryCollectionName, "info2");


Console.WriteLine("Get a memory by ID: info2");
Console.WriteLine(getMemoryResult!.Metadata.Text);
Console.WriteLine("------------------");

var searchResults = textMemory.SearchAsync(memoryCollectionName, "Where do I live?", withEmbeddings: true, limit: 1);


Console.WriteLine("Search for a memory: Where do I live?");
await foreach (var result in searchResults)
{
    Console.WriteLine(result.Metadata.Text);
}

Console.WriteLine("--------------------");