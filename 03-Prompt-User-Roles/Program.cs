using Azure.Core;
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

Console.Clear();

var builder = Kernel.CreateBuilder();

builder.Services
    .AddLogging(loggingBuilder => loggingBuilder.AddDebug().SetMinimumLevel(LogLevel.Trace))
    .AddAzureOpenAIChatCompletion(
        chatCompletionDeploymentName!,
        endpoint!,
        apiKey!);

var kernel = builder.Build();

const string history = """
                       <message role="user">I hate sending emails, no one ever reads them.</message>
                       <message role="assistant">I'm sorry to hear that. Messages may be a better way to communicate.</message>
                       """;

const string prompt = """
                      <message role="system">Instructions: What is the intent of this request?
                      If you don't know the intent, don't guess; instead respond with "Unknown".
                      Choices: SendEmail, SendMessage, CompleteTask, CreateDocument, Unknown.</message>

                      <message role="user">Can you send a very quick approval to the marketing team?</message>
                      <message role="system">Intent:</message>
                      <message role="assistant">SendMessage</message>

                      <message role="user">Can you send the full update to the marketing team?</message>
                      <message role="system">Intent:</message>
                      <message role="assistant">SendEmail</message>

                      {{$history}}
                      
                      <message role="user">{{$request}}</message>
                      <message role="system">Intent:</message>
                      """;

var inlineFunction = kernel.CreateFunctionFromPrompt(prompt);

var kernelArguments = new KernelArguments
{
    { "request", "Compose a document" },
    { "history", history }
};

var result = await kernel.InvokeAsync(inlineFunction, kernelArguments);

Console.WriteLine(result);