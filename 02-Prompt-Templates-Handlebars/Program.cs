using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;

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

var kernel = builder.Build();

List<string> coloursList = ["Red", "Green", "Blue"];

var handlebarsPrompt = @"
            Please describe each the following colours in 10 words or less
             {{#each colours}}
                 {{this}}
             {{/each}}
        ";

var promptTemplateConfig = new PromptTemplateConfig
{
    Template = handlebarsPrompt,
    TemplateFormat = "handlebars"
};

var handlebarsFunction = kernel.CreateFunctionFromPrompt(promptTemplateConfig, new HandlebarsPromptTemplateFactory());

var result= await kernel.InvokeAsync(
    handlebarsFunction,
    new() {
        { "colours", coloursList }
    }
);


Console.WriteLine(result);