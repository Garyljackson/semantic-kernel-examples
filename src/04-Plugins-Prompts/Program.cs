using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using System.Reflection;

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("AppSettings.json", optional: true, reloadOnChange: true)
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

var kernel = builder.Build();

var pluginFolder = Path.Combine(Directory.GetCurrentDirectory(), "Plugins");

var plugins = kernel.CreatePluginFromPromptDirectory(pluginFolder);

const string request = "Send an email to the marketing team";

var kernelArguments = new KernelArguments 
    { { "request", request } };

var result = await kernel.InvokeAsync(plugins["GetIntent"], kernelArguments);

Console.WriteLine($"Get Intent Prompt: {request}");

Console.WriteLine(result);