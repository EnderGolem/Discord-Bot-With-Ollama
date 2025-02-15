using DiscordBot.Classes;
using DiscordBot.OllamaClasses;
using Newtonsoft.Json;
using System.Text;

namespace DiscordBot;

public class OllamaClientGenerate : OllamaClientBase
{
    private const int _maxCountOfMessagesInHistory = 20;



    private Dictionary<ulong, Queue<string>> historyChatsOfChannel = new();

    public OllamaClientGenerate(string apiUrl, string name, string systemPrompt, string prompt, string model) : base(apiUrl, name, systemPrompt, prompt, model)
    {
    }

    protected override string? ParseResponseBody(string responseBody)
    {
        var lines = responseBody.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var sb = new StringBuilder();

        foreach (var line in lines)
        {
            var obj = JsonConvert.DeserializeObject<OllamaGenerateResponse>(line);
            if (obj != null)
            {
                sb.Append(obj.Response);
            }
        }
        return sb.ToString();
    }

    private string GetMessageFromHistory(ulong channel)
    {
        StringBuilder sb = new StringBuilder();

        foreach (var message in historyChatsOfChannel[channel].SkipLast(1))
            sb.AppendLine(message);
        sb.AppendLine($"Последнее сообщение\n{historyChatsOfChannel[channel].Last()}");
        return sb.ToString();
    }

    public override object CreateRequestContent(ulong channel)
    {
        var history = GetMessageFromHistory(channel);
        var requestContent = new { model = _model, system = _system, prompt = _prompt.Replace("#InnerPrompt", history) };

        return requestContent;
    }

    public override void AddToHistory(ulong channel, string author, string message)
    {
        if (historyChatsOfChannel.ContainsKey(channel))
            historyChatsOfChannel[channel].Enqueue($"{author}: {message}");
        else
            historyChatsOfChannel[channel] = new Queue<string>(new[] { $"{author}: {message}" });


        if (historyChatsOfChannel[channel].Count > _maxCountOfMessagesInHistory)
            historyChatsOfChannel[channel].Dequeue();
    }
}

