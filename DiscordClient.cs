using Discord.WebSocket;
using Discord;
using System.Text;
using System.Linq;

namespace DiscordBot;

internal class DiscordClient
{
    private const int _maxCountOfMessagesInHistory = 7;

    private readonly OllamaClient _clientOllama;
    private readonly DiscordSocketClient _clientSocketDiscord;
    private readonly string _token;

    private Dictionary<string, Queue<string>> historyChatsOfChannel = new(); 

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

        if (arg.Content.Contains("/ping"))
        {
            await arg.Channel.SendMessageAsync("Pong!");
            return;
        }

        AddToHistory(arg.Channel.Name, arg.Author.Username, arg.Content);
    
        _ = Task.Run(() => GenerateAndSendMessage(arg.Channel, GetMessageFromHistory(arg.Channel.Name)));
    }

    private async Task GenerateAndSendMessage(ISocketMessageChannel channel, string message)
    {
        //TODO вынести это в отдельную обработку, чтобы он мог продолжать слушать, даже когда генерирует ответ
        var answer = await _clientOllama.GetResponseAsync(message);
        AddToHistory(channel.Name, "Лами", answer);
        Console.WriteLine($"Sending message: {answer}");
        await channel.SendMessageAsync(answer);
    }

    private string GetMessageFromHistory(string channel) 
    {
        StringBuilder sb = new StringBuilder();

        foreach(var message in historyChatsOfChannel[channel].SkipLast(1))        
            sb.AppendLine(message);
        sb.AppendLine($"Последнее сообщение\n{historyChatsOfChannel[channel].Last()}");
        return sb.ToString();
    }

    private void AddToHistory(string channel, string author, string message)
    {
        if(historyChatsOfChannel.ContainsKey(channel))
             historyChatsOfChannel[channel].Enqueue($"{author}: {message}");
        else
            historyChatsOfChannel[channel] = new Queue<string>(new[] { $"{author}: {message}" });


        if (historyChatsOfChannel[channel].Count > _maxCountOfMessagesInHistory)        
            historyChatsOfChannel[channel].Dequeue();       
    }
}
