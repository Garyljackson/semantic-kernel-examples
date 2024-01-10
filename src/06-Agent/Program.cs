using _06_Agent.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
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

builder.Plugins.AddFromType<AuthorEmailPlanner>();
builder.Plugins.AddFromType<EmailPlugin>();

var kernel = builder.Build();

var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

var chatMessages = new ChatHistory("""
                                   You are a friendly assistant who likes to follow the rules. You will complete required steps
                                   and request approval before taking any consequential actions. If the user doesn't provide
                                   enough information for you to complete a task, you will keep asking questions until you have
                                   enough information to complete the task.
                                   """);

OpenAIPromptExecutionSettings openAiPromptExecutionSettings = new()
{
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
};

// Start the conversation
while (true)
{
    // Get user input
    Console.Write("User > ");
    chatMessages.AddUserMessage(Console.ReadLine()!);

    var result = chatCompletionService.GetStreamingChatMessageContentsAsync(
        chatMessages,
        executionSettings: openAiPromptExecutionSettings,
        kernel: kernel);

    // Stream the results
    var fullMessage = "";

    await foreach (var content in result)
    {
        if (content.Role.HasValue)
        {
            Console.Write("Assistant > ");
        }

        Console.Write(content.Content);
        fullMessage += content.Content;
    }

    Console.WriteLine();

    // Add the message from the agent to the chat history
    chatMessages.AddAssistantMessage(fullMessage);
}