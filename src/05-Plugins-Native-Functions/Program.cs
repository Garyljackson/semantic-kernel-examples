using _04_Native_Functions.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using System.Reflection;

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
    .Build();

var azureOpenAiSettings = configuration.GetSection("AzureOpenAi");

var endpoint = azureOpenAiSettings["Endpoint"];
var apiKey = azureOpenAiSettings["ApiKey"];
var chatCompletionDeploymentName = azureOpenAiSettings["ChatCompletionDeploymentName"];

Console.Clear();

var builder = Kernel.CreateBuilder();

builder.Services
    .AddLogging(loggingBuilder => loggingBuilder.AddDebug().SetMinimumLevel(LogLevel.Trace))
    .AddAzureOpenAIChatCompletion(
        chatCompletionDeploymentName!,
        endpoint!,
        apiKey!);

builder.Plugins.AddFromType<MathPlugin>();

var kernel = builder.Build();

var sqrtAnswer = await kernel.InvokeAsync<double>(
    pluginName: "MathPlugin", 
    functionName:"Sqrt",
    new KernelArguments { { "number1", 12 } }
);

Console.WriteLine($"The square root of 12 is {sqrtAnswer}.");

var kernelArguments = new KernelArguments
{
    { "number1", 12 },
    { "number2", 5 }
};

var addAnswer = await kernel.InvokeAsync<double>(
    pluginName: "MathPlugin",
    functionName: "Add",
    kernelArguments
);

Console.WriteLine($"5 added to 12 is {addAnswer}.");