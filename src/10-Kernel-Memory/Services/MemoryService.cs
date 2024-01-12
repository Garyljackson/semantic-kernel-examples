using _10_Kernel_Memory.Models;
using Microsoft.KernelMemory;

namespace _10_Kernel_Memory.Services;

public class MemoryService : IMemoryService
{
    private IKernelMemory kernelMemory;

    public MemoryService(IConfiguration configuration)
    {
        var endpoint = configuration["AzureOpenAi:Endpoint"]!;
        var apiKey = configuration["AzureOpenAi:ApiKey"]!;

        var deploymentChatName = configuration["AzureOpenAi:ChatCompletionDeploymentName"]!;
        var deploymentEmbeddingName = configuration["AzureOpenAi:EmbeddingsDeploymentName"]!;
        
        var embeddingConfig = new AzureOpenAIConfig
        {
            APIKey = apiKey,
            Deployment = deploymentEmbeddingName,
            Endpoint = endpoint,
            APIType = AzureOpenAIConfig.APITypes.EmbeddingGeneration,
            Auth = AzureOpenAIConfig.AuthTypes.APIKey
        };

        var chatConfig = new AzureOpenAIConfig
        {
            APIKey = apiKey,
            Deployment = deploymentChatName,
            Endpoint = endpoint,
            APIType = AzureOpenAIConfig.APITypes.ChatCompletion,
            Auth = AzureOpenAIConfig.AuthTypes.APIKey
        };

        var path = Path.Combine($"{Directory.GetCurrentDirectory()}//Memory");

        kernelMemory = new KernelMemoryBuilder()
            .WithAzureOpenAITextGeneration(chatConfig)
            .WithAzureOpenAITextEmbeddingGeneration(embeddingConfig)
            //.WithSimpleVectorDb()
            .WithSimpleVectorDb(path)
            .Build<MemoryServerless>();
    }

    public async Task<bool> StoreText(string text)
    {
        try
        {
            await kernelMemory.ImportTextAsync(text);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> StoreFile(string path, string filename)
    {
        try
        {
            await kernelMemory.ImportDocumentAsync(path, filename);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> StoreWebsite(string url)
    {
        try
        {
            await kernelMemory.ImportWebPageAsync(url);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<KernelResponse> AskQuestion(string question)
    {
        var answer = await kernelMemory.AskAsync(question);

        var response = new KernelResponse
        {
            Answer = answer.Result,
            Citations = answer.RelevantSources
        };

        return response;
    }
}