using DiscordBot.OllamaClasses;
using Microsoft.Extensions.Configuration;

namespace DiscordBot;

//TODO попытаться запустить на сервере
class Program
{
    private static string _token = "MTMxNTc0MDkwMTQ2MDU0NTU1Ng.G4XWE3.NiaVNuVDRyONFxP5ve_92d8oNMPO12Rr3oLUf8";

    static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        string token = configuration["Discord:Token"]!;

        string name = "Лами";
        string system = $"Ты {name} чат бот." +
                        $"1) Учитывай историю чтобы сохранить контекст." +
                        $"2) Веди себя как пользователь дискорд серверов." +
                        $"3) Пиши коротко и дружелюбно, используя 1–2 предложения." +
                        $"4) Отвечай всегда на русском языке.";
        string prompt = $"Вот история переписки: {"#InnerPrompt"}" +
                            $"Ответь на последнее сообщение, как {name}.";

        IOllamaChat clientOllama = new OllamaClient("http://localhost:11434/api/generate", "Лами", system, prompt);
        DiscordClient discordClient = new DiscordClient(_token, clientOllama);

        await discordClient.Initialize();


        var _ = RunLoopAsync(() => discordClient.ProcessQueueOfMessages(), 1000);

        await Task.Delay(-1);
    }

    //TODO Переписать на IHostedService , как я понял это что-то связанное с BackgroundService и выглядит современно, со слов ИИ
    private static async Task RunLoopAsync(Action action, int repeatTime, CancellationToken token = default)
    {
        while (!token.IsCancellationRequested)
        {
            action();
            await Task.Delay(repeatTime, token);
        }
    }
}
