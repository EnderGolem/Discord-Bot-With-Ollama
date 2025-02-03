
using DiscordBot.Classes;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace DiscordBot.OllamaClasses;

public class OllamaClientChat : IOllamaClient
{
    private const int _maxCountOfMessagesInHistory = 20;

    private readonly string _apiUrl;
    private readonly string _system;
    private readonly string _prompt;
    private readonly HttpClient _httpClient;
    private readonly Random _random = new();

    private Dictionary<ulong, Queue<(string author, string message)>> historyChatsOfChannel = new();

    public string Name { get; private set; }

    public bool AlwaysResponse { get; set; } = true;


    //TODO сделать отдельную настройку по чатам
    public OllamaClientChat(string apiUrl, string name, string system, string prompt)
    {
        _httpClient = new HttpClient();
        _apiUrl = apiUrl;
        _system = system;
        _prompt = prompt;

        Name = name;
    }

    public async Task<string?> GetResponseAsync(object requestContent)
    {
        if (!AlwaysResponse && _random.Next(0, 101) > 10)
            return null;

        var jsonContent = JsonConvert.SerializeObject(requestContent);
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        try
        {
            HttpResponseMessage response = await _httpClient.PostAsync(_apiUrl, httpContent);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error: {response.StatusCode} - {response.ReasonPhrase}");
            }

            string responseBody = await response.Content.ReadAsStringAsync();

            var responseObject = JsonConvert.DeserializeObject<OllamaChatResponse>(responseBody);

            return responseObject!.Message.Content;
        }
        catch (Exception ex)
        {
            return $"An error occurred: {ex.Message}";
        }
    }


    public void AddToHistory(ulong channel, string author, string message)
    {
        if (historyChatsOfChannel.ContainsKey(channel))
            historyChatsOfChannel[channel].Enqueue((author,message));
        else
            historyChatsOfChannel[channel] = new Queue<(string author, string message)>(new[] { (author, message) });


        if (historyChatsOfChannel[channel].Count > _maxCountOfMessagesInHistory)
            historyChatsOfChannel[channel].Dequeue();
    }

    public object CreateRequestContent(ulong channel)
    {
        Queue<(string author, string message)> historyChat = historyChatsOfChannel[channel];

        var messages = historyChat.Select(m => new
        {
            role = m.author == Name ? "assistant" : "user",
            content = m.author == Name ? m.message : $"{m.author}:{m.message}"
        }).ToArray();

        if (!string.IsNullOrEmpty(_system))
            messages = (new[] { new { role = "system", content = _system } }).Concat(messages).ToArray();


        object requestContent = new { model = "llama3",
            messages = messages,
            stream = false
        };

        return requestContent;
    }

}
