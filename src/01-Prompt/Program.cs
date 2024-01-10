using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

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

var kernel = builder.Build();

var request = "The stars are so bright tonight";

var prompt = $"""
              Rewrite the following in the style of Sherlock Holmes

              {request}
              """;


Console.WriteLine("Sherlock Holmes Prompt:");
Console.WriteLine(await kernel.InvokePromptAsync(prompt));
Console.WriteLine("----------------------");

request = "Send an email to the marketing team";

prompt = $"""
          Instructions: What is the intent of this request?
          Choices: SendEmail, SendMessage, CompleteTask, CreateDocument.
          User Input: {request}

          Intent:
          """;


Console.WriteLine("Get Intent Prompt:");
Console.WriteLine(await kernel.InvokePromptAsync(prompt));
Console.WriteLine("-----------------");