using _04_Native_Functions.Plugins;
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

builder.Plugins.AddFromType<MathPlugin>();

var kernel = builder.Build();

var sqrtAnswer = await kernel.InvokeAsync<double>(
    pluginName: "MathPlugin", 
    functionName:"Sqrt",
    new KernelArguments { { "number1", 12 } }
);

Console.WriteLine($"The square root of 12 is {sqrtAnswer}.");


var addAnswer = await kernel.InvokeAsync<double>(
    pluginName: "MathPlugin",
    functionName: "Add",
    new KernelArguments { { "number1", 12 }, { "number2", 5 } }
);

Console.WriteLine($"5 added to 12 is {addAnswer}.");