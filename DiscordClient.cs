using Discord;
using Discord.WebSocket;
using DiscordBot.Classes;
using DiscordBot.OllamaClasses;

namespace DiscordBot;

internal class DiscordClient
{

    private readonly IOllamaClient _clientOllama;
    private readonly DiscordSocketClient _clientSocketDiscord;
    private readonly string _token;

    private Queue<MessageData> _queueMessages = new();
    private Dictionary<ulong, bool> _alwaysResponse = new();

    public DiscordClient(string token, IOllamaClient ollamaClient)
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

        if (!_alwaysResponse.ContainsKey(arg.Channel.Id))
            _alwaysResponse[arg.Channel.Id] = true;

        Console.WriteLine($"Received: {arg.Content} from {arg.Author.Username} channel: {arg.Channel.Name}");


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

        if (arg.Content.Contains("/alwaysChat"))
        {
            _alwaysResponse[arg.Channel.Id] = !_alwaysResponse[arg.Channel.Id];
            await arg.Channel.SendMessageAsync(text: $"I {(_alwaysResponse[arg.Channel.Id] ? "always respond now" : "respond only when mentioned")}", messageReference: messageReference);
            return;
        }

        _clientOllama.AddToHistory(arg.Channel.Id, arg.Author.Username, arg.Content);
        if (_alwaysResponse[arg.Channel.Id] || CheckBotMentioned(arg))
            _queueMessages.Enqueue(new(DateTime.UtcNow, arg.Content.Replace(_clientSocketDiscord.CurrentUser.Id.ToString(), _clientOllama.Name), arg.Channel, messageReference));
    }

    public void ProcessQueueOfMessages()
    {
        if (_queueMessages.Any())
        {
            var messageData = _queueMessages.Dequeue();

            if (messageData.Timestamp.AddSeconds(20) < DateTime.UtcNow)
                return;

            //TODO Разобраться почему тут надо ставить .GetAwaiter().GetResult(), а не result  и вообще каким образом образуется deadLock
            var answer = _clientOllama.GetResponseAsync(_clientOllama.CreateRequestContent(messageData.Channel.Id)).GetAwaiter().GetResult();
            if (string.IsNullOrEmpty(answer))
                return;

            Console.WriteLine($"Sending message: {answer}");
            _clientOllama.AddToHistory(messageData.Channel.Id, _clientOllama.Name, answer);
            messageData.Channel.SendMessageAsync(text: answer, messageReference: messageData.Reference);
        }
    }

    private bool CheckBotMentioned(SocketMessage arg)
    {
        return arg.MentionedUsers.Any(user => user.Id == _clientSocketDiscord.CurrentUser.Id);
    }
}
