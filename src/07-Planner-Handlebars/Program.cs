using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using System.Reflection;
using _07_Planner.Plugins;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Planning.Handlebars;

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

builder.Plugins.AddFromType<MathPlugin>();

var kernel = builder.Build();

ChatHistory history = [];

var planner = new HandlebarsPlanner(new HandlebarsPlannerOptions() { AllowLoops = true });

var plan = await planner.CreatePlanAsync(kernel, "How many slices of pizza would everyone get if you split 12 slices equally among 3 people");

Console.WriteLine("Plan steps");
Console.WriteLine(plan);
Console.WriteLine("----------");

var result = (await plan.InvokeAsync(kernel, [])).Trim();

Console.WriteLine("Result");
Console.WriteLine(result);