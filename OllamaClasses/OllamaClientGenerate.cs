using DiscordBot.Classes;
using DiscordBot.OllamaClasses;
using Newtonsoft.Json;
using System.Text;

namespace DiscordBot;

public class OllamaClientGenerate : OllamaClientBase
{
    private readonly string _lastMessageFormatting;
    private Dictionary<ulong, Queue<string>> historyChatsOfChannel = new();
        
    public OllamaClientGenerate(string apiUrl, string name, string systemPrompt, string prompt, string model, 
        int maxCountOfMessagesInHistory, string lastMessageFormatting = "") :
        base(apiUrl, name, systemPrompt, prompt, model, maxCountOfMessagesInHistory)
    {
        _lastMessageFormatting = lastMessageFormatting;
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

        if (!string.IsNullOrEmpty(_lastMessageFormatting))
            sb.AppendLine(_lastMessageFormatting.Replace("#lastMessage", historyChatsOfChannel[channel].Last()));
        else
            sb.AppendLine(historyChatsOfChannel[channel].Last());

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

