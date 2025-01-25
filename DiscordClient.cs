using Discord.WebSocket;
using Discord;
using System.Text;
using System.Linq;
using DiscordBot.Classes;

namespace DiscordBot;

internal class DiscordClient
{

    private readonly OllamaClient _clientOllama;
    private readonly DiscordSocketClient _clientSocketDiscord;
    private readonly string _token;

    private Queue<MessageData> _queueMessages = new();


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
        _queueMessages.Enqueue(new(DateTime.UtcNow, arg.Content, arg.Channel, messageReference));
    }

    public void ProcessQueueOfMessages()
    {
        if (_queueMessages.Any())
        {
            var messageData = _queueMessages.Dequeue();

            if (messageData.Timestamp.AddSeconds(20) < DateTime.UtcNow)
                return;

            var answer = _clientOllama.GetResponseAsync(_clientOllama.GetMessageFromHistory(messageData.Channel.Id)).Result;
            
            Console.WriteLine($"Sending message: {answer}");
            _clientOllama.AddToHistory(messageData.Channel.Id, _clientOllama.Name, answer);
            messageData.Channel.SendMessageAsync(text: answer, messageReference: messageData.Reference);
        }
    }
}
