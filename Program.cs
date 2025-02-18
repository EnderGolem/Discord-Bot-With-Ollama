using DiscordBot.Enums;
using DiscordBot.OllamaClasses;
using Microsoft.Extensions.Configuration;

namespace DiscordBot;

class Program
{
    static async Task Main(string[] args)
    {

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        string discordToken = configuration["Discord:Token"]
            ?? throw new InvalidOperationException("Missing 'Discord:Token' value in configuration.");
        string ollamaName = configuration["Ollama:Name"]
            ?? throw new InvalidOperationException("Missing 'Ollama:Name' value in configuration.");
        string ollamaSystemPrompt = configuration["Ollama:SystemPromt"]
            ?? throw new InvalidOperationException("Missing 'Ollama:SystemPromt' value in configuration.");
        string ollamaPrompt = configuration["Ollama:Promt"]
            ?? throw new InvalidOperationException("Missing 'Ollama:Promt' value in configuration.");
        string ollamaModelTypeStr = configuration["Ollama:ModelType"]
            ?? throw new InvalidOperationException("Missing 'Ollama:ModelType' value in configuration.");
        string ollamaURL = configuration["Ollama:OllamaURL"]
            ?? throw new InvalidOperationException("Missing 'Ollama:OllamaURL' value in configuration.");
        string ollamaVersion = configuration["Ollama:ModelVersion"]
            ?? throw new InvalidOperationException("Missing 'Ollama:ModelVersion' value in configuration.");
        string ollamaMemoryStr = configuration["Ollama:MessageMemorisedCount"]
            ?? throw new InvalidOperationException("Missing 'Ollama:MessageMemorisedCount' value in configuration.");
        string ollamaLastMessageFormating = configuration["Ollama:LastMessageFormatting"]
            ?? "";

        if (!int.TryParse(ollamaMemoryStr, out int ollamaMemory))
        {
            throw new InvalidOperationException($"Invalid MessageMemorisedCount value in configuration: {ollamaMemory}");
        }

        if (!Enum.TryParse(ollamaModelTypeStr, true, out ModelType modelType))
        {
            throw new InvalidOperationException($"Invalid ModelType value in configuration: {ollamaModelTypeStr}");
        }


        IOllamaClient clientOllama;
        switch (modelType)
        {
            case ModelType.Generate:
                clientOllama = new OllamaClientGenerate(ollamaURL, ollamaName, ollamaSystemPrompt, ollamaPrompt, ollamaVersion, ollamaMemory, ollamaLastMessageFormating);
                break;
            case ModelType.Chat:
                clientOllama = new OllamaClientChat(ollamaURL, ollamaName, ollamaSystemPrompt, ollamaPrompt, ollamaVersion, ollamaMemory);
                break;
            default:
                throw new InvalidOperationException("Invalid system state");
        }

        Console.WriteLine("Loaded the model with the following parameters\n" +
                          $"Type: {modelType}\n" +
                          $"Ollama model: {ollamaVersion}\n" +
                          $"Name: {ollamaName}\n" +
                          $"System prompt: {ollamaSystemPrompt}\n" +
                          $"Prompt: {ollamaPrompt}\n" +
                          $"Last message formatting: {ollamaLastMessageFormating}\n" +
                          $"Max number of messages in memory: {ollamaMemory}\n" +
                          $"URL: {ollamaURL}\n");

        DiscordClient discordClient = new DiscordClient(discordToken, clientOllama);


        await discordClient.Initialize();
        
        var _ = RunLoopAsync(() => discordClient.ProcessQueueOfMessages(), 1000);

        await Task.Delay(-1);
    }

    private static async Task RunLoopAsync(Action action, int repeatTime, CancellationToken token = default)
    {
        while (!token.IsCancellationRequested)
        {
            action();
            await Task.Delay(repeatTime, token);
        }
    }
}
