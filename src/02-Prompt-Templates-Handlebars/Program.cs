using Azure.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;
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

// Note how the request placeholder doesn't have the dollar $ sign anymore for handlebar templates

var handlebarsPrompt = """
                       Instructions: What is the intent of this request?
                       {{#each intentOptions}}
                          {{this}}
                       {{/each}}

                       User Input: {{request}}

                       Intent:
                       """;

var promptTemplateConfig = new PromptTemplateConfig
{
    Template = handlebarsPrompt,
    TemplateFormat = "handlebars"
};

var handlebarsFunction = kernel.CreateFunctionFromPrompt(promptTemplateConfig, new HandlebarsPromptTemplateFactory());

var intentOptions = new List<string> { "SendEmail", "SendMessage", "CompleteTask", "CreateDocument" };

const string request = "Send an email to the marketing team";

var kernelArguments = new KernelArguments
{
    { "request", request },
    { "intentOptions", intentOptions }
};

var result = await kernel.InvokeAsync(
    handlebarsFunction,
    kernelArguments
);


Console.WriteLine($"Get Intent Prompt: {request}");

Console.WriteLine(result);