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
                ?? throw new InvalidOperationException("Отсутствует значение 'Discord:Token' в конфигурации.");
        string ollamaName = configuration["Ollama:Name"]
                ?? throw new InvalidOperationException("Отсутствует значение 'Ollama:Name' в конфигурации.");
        string ollamaSystemPrompt = configuration["Ollama:SystemPromt"]
            ?? throw new InvalidOperationException("Отсутствует значение 'Ollama:SystemPromt' в конфигурации.");
        string ollamaPrompt = configuration["Ollama:Promt"]
            ?? throw new InvalidOperationException("Отсутствует значение 'Ollama:Promt' в конфигурации.");
        string ollamaModelTypeStr = configuration["Ollama:ModelType"]
            ?? throw new InvalidOperationException("Отсутствует значение 'Ollama:ModelType' в конфигурации.");
        string ollamaURL = configuration["Ollama:OllamaURL"]
            ?? throw new InvalidOperationException("Отсутствует значение 'Ollama:OllamaURL' в конфигурации.");
        string ollamaVersion = configuration["Ollama:ModelVersion"]
            ?? throw new InvalidOperationException("Отсутствует значение 'Ollama:ModelVersion' в конфигурации.");
        string ollamaMemoryStr = configuration["Ollama:MessageMemorisedCount"]
            ?? throw new InvalidOperationException("Отсутствует значение 'Ollama:MessageMemorisedCount' в конфигурации.");


        if (!int.TryParse(ollamaMemoryStr, out int ollamaMemory))
        { 
            throw new InvalidOperationException($"Некорректное значение MessageMemorisedCount в конфигурации: {ollamaMemory}");
        }

        if (!Enum.TryParse(ollamaModelTypeStr, true, out ModelType modelType))
        {
            throw new InvalidOperationException($"Некорректное значение ModelType в конфигурации: {ollamaModelTypeStr}");
        }


        IOllamaClient clientOllama;
        switch (modelType)
        {
            case ModelType.Generate:
                clientOllama = new OllamaClientGenerate(ollamaURL, ollamaName, ollamaSystemPrompt, ollamaPrompt, ollamaVersion, ollamaMemory);
                break;
            case ModelType.Chat:
                clientOllama = new OllamaClientChat(ollamaURL, ollamaName, ollamaSystemPrompt, ollamaPrompt, ollamaVersion, ollamaMemory);
                break;
            default:
                throw new InvalidOperationException("Не допустимое состояние системы");
        }

        Console.WriteLine("Загружена модель со следующими параметрами\n" +
                        $"Тип: {modelType}\n" +
                        $"Модель ollama: {ollamaVersion}\n" +
                        $"Имя: {ollamaName}\n" +
                        $"Системный промт: {ollamaSystemPrompt}\n" +
                        $"Промт: {ollamaPrompt}\n" +
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
