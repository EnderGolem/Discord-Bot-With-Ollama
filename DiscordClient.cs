using Discord.WebSocket;
using Discord;
using Newtonsoft.Json.Linq;

namespace DiscordBot;

internal class DiscordClient
{
    private readonly OllamaClient _clientOllama;
    private readonly DiscordSocketClient _clientSocketDiscord;
    private readonly string _token;

    public DiscordClient(string token, OllamaClient ollamaClient)
    {

        _clientOllama = ollamaClient;
        _token = token;

        _clientSocketDiscord = new DiscordSocketClient(new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
        });
    }


    public async Task Initialize()
    {
        _clientSocketDiscord.Log += Log;
        _clientSocketDiscord.Ready += Ready;
        _clientSocketDiscord.MessageReceived += MessageReceived;

        await _clientSocketDiscord.LoginAsync(TokenType.Bot, _token);
        await _clientSocketDiscord.StartAsync();
    }

    private Task Log(LogMessage log)
    {
        Console.WriteLine(log);
        return Task.CompletedTask;
    }

    private Task Ready()
    {
        Console.WriteLine("Bot is connected and ready.");
        return Task.CompletedTask;
    }

    private async Task MessageReceived(SocketMessage arg)
    {
        if (arg.Author.IsBot)
            return;

        Console.WriteLine($"Received: {arg.Content}");
        if (arg.Content.Contains("/ping"))
        {
            await arg.Channel.SendMessageAsync("Pong!");
            return;
        }

        _ = Task.Run(() => GenerateAndSendMessage(arg));
    }

    private async Task GenerateAndSendMessage(SocketMessage arg)
    {
        //TODO вынести это в отдельную обработку, чтобы он мог продолжать слушать, даже когда генерирует ответ
        var answer = await _clientOllama.GetResponseAsync(arg.Content);

        Console.WriteLine($"Sending message: {answer}");
        await arg.Channel.SendMessageAsync(answer);
    }
}
