using Discord.WebSocket;
using Discord;
using System.Text;
using System.Linq;

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

        Console.WriteLine($"Received: {arg.Content} from {arg.Author.Username}");



        var messageReference = new MessageReference(
                messageId: arg.Id,
                channelId: arg.Channel.Id,
                guildId: (arg.Channel as SocketGuildChannel)?.Guild.Id
            );

        if (arg.Content.Contains("/ping"))
        {

            await arg.Channel.SendMessageAsync(text: $"pong!", messageReference: messageReference);
            return;
        }

       _clientOllama.AddToHistory(arg.Channel.Id, arg.Author.Username, arg.Content);


        _ = Task.Run(() => GenerateAndSendMessage(arg.Channel, _clientOllama.GetMessageFromHistory(arg.Channel.Id), messageReference));
    }

    private async Task GenerateAndSendMessage(ISocketMessageChannel channel, string message, MessageReference messageReference)
    {
        //TODO вынести это в отдельную обработку, чтобы он мог продолжать слушать, даже когда генерирует ответ
        var answer = await _clientOllama.GetResponseAsync(message);
        _clientOllama.AddToHistory(channel.Id, _clientOllama.Name, answer);
        Console.WriteLine($"Sending message: {answer}");
        await channel.SendMessageAsync(text: answer, messageReference: messageReference);
    }
}
