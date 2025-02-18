using Newtonsoft.Json;
using System.Text;

namespace DiscordBot.OllamaClasses;

public abstract class OllamaClientBase : IOllamaClient
{
    protected readonly int _maxCountOfMessagesInHistory;
    protected readonly string _apiUrl;
    protected readonly string _system;
    protected readonly string _prompt;
    protected readonly string _model;
    protected readonly HttpClient _httpClient;

    public string Name { get; }

    protected OllamaClientBase(string apiUrl, string name, string systemPrompt, string prompt, string model, int maxCountOfMessagesInHistory)
    {
        _apiUrl = apiUrl;
        _system = systemPrompt;
        _prompt = prompt;
        _model = model;
        _maxCountOfMessagesInHistory = maxCountOfMessagesInHistory;
        _httpClient = new HttpClient();

        Name = name;
    }

    public async Task<string?> GetResponseAsync(object requestContent)
    {
        try
        {
            var jsonContent = JsonConvert.SerializeObject(requestContent);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            using HttpResponseMessage response = await _httpClient.PostAsync(_apiUrl, httpContent)
                                                               .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error: {response.StatusCode} - {response.ReasonPhrase}");
            }

            string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return ParseResponseBody(responseBody);
        }
        catch (Exception ex)
        {
            return $"An error occurred: {ex.Message}";
        }
    }

    protected abstract string? ParseResponseBody(string responseBody);

    public abstract object CreateRequestContent(ulong channel);

    public abstract void AddToHistory(ulong channel, string author, string message);
}

