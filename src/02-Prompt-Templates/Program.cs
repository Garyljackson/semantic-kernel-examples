﻿using Microsoft.Extensions.Configuration;
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
var chatCompletionModelId = azureOpenAiSettings["ChatCompletionModelId"];

Console.Clear();

var builder = Kernel.CreateBuilder();

builder.Services
    .AddLogging(loggingBuilder => loggingBuilder.AddDebug().SetMinimumLevel(LogLevel.Trace))
    .AddAzureOpenAIChatCompletion(
        chatCompletionDeploymentName!,
        endpoint!,
        apiKey!);

var kernel = builder.Build();

var prompt = """
          Instructions: What is the intent of this request?
          Choices: SendEmail, SendMessage, CompleteTask, CreateDocument.
          User Input: {{$request}}

          Intent:
          """;

var inlineFunction = kernel.CreateFunctionFromPrompt(prompt);

var kernelArguments = new KernelArguments 
    { { "request", "Send an email to the marketing team" } };

var result = await kernel.InvokeAsync(inlineFunction, kernelArguments);

Console.WriteLine(result);