using Discord;
using DiscordBot.Classes;
using Newtonsoft.Json;
using System.Text;

namespace DiscordBot;

public class OllamaClient
{
    private const int _maxCountOfMessagesInHistory = 7;

    private readonly HttpClient _httpClient;
    private readonly string _apiUrl;
    private readonly string _system;
    private readonly string _prompt;

    private Dictionary<ulong, Queue<string>> historyChatsOfChannel = new();

    public string Name { get; private set; }

    public OllamaClient(string apiUrl, string name, string system, string prompt)
    {
        _httpClient = new HttpClient();
        _apiUrl = apiUrl;
        _system = system;
        _prompt = prompt;

        Name = name;
    }

    public async Task<string> GetResponseAsync(string prompt)
    {

        var requestContent = new { model = "llama3", system = _system, prompt = _prompt.Replace("#InnerPrompt", prompt) };

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
            string[] lines = responseBody.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var responseObjects = new List<OllamaResponse>();

            foreach (var line in lines)
            {
                var responseObject = JsonConvert.DeserializeObject<OllamaResponse>(line);
                if (responseObject != null)
                {
                    responseObjects.Add(responseObject);
                }
            }

            var fullResponse = new StringBuilder();
            foreach (var obj in responseObjects)
            {
                fullResponse.Append(obj.Response);
            }

            return fullResponse.ToString();
        }
        catch (Exception ex)
        {
            return $"An error occurred: {ex.Message}";
        }
    }
    public string GetMessageFromHistory(ulong channel)
    {
        StringBuilder sb = new StringBuilder();

        foreach (var message in historyChatsOfChannel[channel].SkipLast(1))
            sb.AppendLine(message);
        sb.AppendLine($"Последнее сообщение\n{historyChatsOfChannel[channel].Last()}");
        return sb.ToString();
    }

    public void AddToHistory(ulong channel, string author, string message)
    {
        if (historyChatsOfChannel.ContainsKey(channel))
            historyChatsOfChannel[channel].Enqueue($"{author}: {message}");
        else
            historyChatsOfChannel[channel] = new Queue<string>(new[] { $"{author}: {message}" });


        if (historyChatsOfChannel[channel].Count > _maxCountOfMessagesInHistory)
            historyChatsOfChannel[channel].Dequeue();
    }
}

