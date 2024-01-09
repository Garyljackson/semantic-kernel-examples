using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Memory;

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

var azureOpenAiSettings = configuration.GetSection("AzureOpenAi");

var endpoint = azureOpenAiSettings["Endpoint"];
var apiKey = azureOpenAiSettings["ApiKey"];
var embeddingsDeploymentName = azureOpenAiSettings["EmbeddingsDeploymentName"];

Console.Clear();

var memoryBuilder = new MemoryBuilder()
    .WithAzureOpenAITextEmbeddingGeneration(
        embeddingsDeploymentName!,
        endpoint!,
        apiKey!)
    .WithMemoryStore(new VolatileMemoryStore());

var memory = memoryBuilder.Build();

const string collectionName = "MyCollection";

await memory.SaveInformationAsync(collectionName, "Oranges are the largest citrus fruit in the world. They originated around 4000 B.C. in Southeast Asia, from where they spread to India.", "1");
await memory.SaveInformationAsync(collectionName, "Despite their name, oranges are not always orange. Depending on the local climate and growing conditions, the skin of the fruit can range from green to yellow to the more familiar orange color.", "2");
await memory.SaveInformationAsync(collectionName, "There are over 600 varieties of oranges. The most common types include the navel orange, the Valencia orange, and the Hamlin orange. Each type has its own unique taste, texture, and color.", "3");

var searchResults = memory.SearchAsync(collectionName, "Are oranges always orange?", withEmbeddings: true, limit: 1);

await foreach (var result in searchResults)
{
    Console.WriteLine(result.Metadata.Text);
}
