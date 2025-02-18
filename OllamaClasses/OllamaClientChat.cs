
using DiscordBot.Classes;
using Newtonsoft.Json;
using System.Text;

namespace DiscordBot.OllamaClasses;

public class OllamaClientChat : OllamaClientBase
{
    private Dictionary<ulong, Queue<(string author, string message)>> historyChatsOfChannel = new();

    public OllamaClientChat(string apiUrl, string name, string systemPrompt, string prompt, string model, int maxCountOfMessagesInHistory) :
        base(apiUrl, name, systemPrompt, prompt, model, maxCountOfMessagesInHistory)
    {
    }

    protected override string? ParseResponseBody(string responseBody)
    {
        var responseObject = JsonConvert.DeserializeObject<OllamaChatResponse>(responseBody);
        return responseObject?.Message?.Content;
    }

    public override object CreateRequestContent(ulong channel)
    {
        Queue<(string author, string message)> historyChat = historyChatsOfChannel[channel];

        var messages = historyChat.Select(m => new
        {
            role = m.author == Name ? "assistant" : "user",
            content = m.author == Name ? m.message : $"{m.author}:{m.message}"
        }).ToArray();

        if (!string.IsNullOrEmpty(_system))
            messages = (new[] { new { role = "system", content = _system } }).Concat(messages).ToArray();


        object requestContent = new
        {
            model = _model,
            messages = messages,
            stream = false
        };

        return requestContent;
    }

    public override void AddToHistory(ulong channel, string author, string message)
    {
        lock (historyChatsOfChannel)
        {
            if (historyChatsOfChannel.ContainsKey(channel))
                historyChatsOfChannel[channel].Enqueue((author, message));
            else
                historyChatsOfChannel[channel] = new Queue<(string author, string message)>(new[] { (author, message) });


            if (historyChatsOfChannel[channel].Count > _maxCountOfMessagesInHistory)
                historyChatsOfChannel[channel].Dequeue();
        }
    }
}
