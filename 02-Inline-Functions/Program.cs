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

var prompt = """
             Rewrite the following in the style of Sherlock Holmes
             
             {{$input}}
             """;

var promptExecutionSettings = new PromptExecutionSettings
{
    ExtensionData = new Dictionary<string, object>
    {
        { "temperature", 0.9 }
    }
};

var inlineFunction = kernel.CreateFunctionFromPrompt(prompt, promptExecutionSettings);

var kernelArguments = new KernelArguments { { "input", "The stars are so bright tonight" } };

var result = await kernel.InvokeAsync(inlineFunction, kernelArguments);

Console.WriteLine(result);