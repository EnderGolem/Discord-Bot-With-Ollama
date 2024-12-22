using Discord;
using Discord.WebSocket;

namespace DiscordBot;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class OllamaClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiUrl;

    public OllamaClient(string apiUrl)
    {
        _httpClient = new HttpClient();
        _apiUrl = apiUrl;
    }

    public async Task<string> GetResponseAsync(string prompt)
    { 

        var requestContent = new { model = "llama3", prompt = $"Отвечай на русском: {prompt}" };
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

    public class OllamaResponse
    {
        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty("response")]
        public string Response { get; set; }

        [JsonProperty("done")]
        public bool Done { get; set; }

        [JsonProperty("done_reason")]
        public string DoneReason { get; set; }

        [JsonProperty("context")]
        public List<int> Context { get; set; }

        [JsonProperty("total_duration")]
        public long TotalDuration { get; set; }

        [JsonProperty("load_duration")]
        public long LoadDuration { get; set; }

        [JsonProperty("prompt_eval_count")]
        public int PromptEvalCount { get; set; }

        [JsonProperty("prompt_eval_duration")]
        public long PromptEvalDuration { get; set; }

        [JsonProperty("eval_count")]
        public int EvalCount { get; set; }

        [JsonProperty("eval_duration")]
        public long EvalDuration { get; set; }
    }
}

class Program
{
    private static DiscordSocketClient _clientDiscord;
    private static OllamaClient _clientOllama;
    private static string _token = "MTMxNTc0MDkwMTQ2MDU0NTU1Ng.G4XWE3.NiaVNuVDRyONFxP5ve_92d8oNMPO12Rr3oLUf8"; // Ваш токен

    static async Task Main(string[] args)
    {
        _clientOllama = new OllamaClient("http://localhost:11434/api/generate");

        _clientDiscord = new DiscordSocketClient(new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
        });

        // Регистрация события при подключении
        _clientDiscord.Log += Log;
        _clientDiscord.Ready += Ready;
        _clientDiscord.MessageReceived += MessageReceived;

        // Подключение бота
        await _clientDiscord.LoginAsync(TokenType.Bot, _token);
        await _clientDiscord.StartAsync();

        // Бот работает в фоновом режиме, пока не будет закрыт
        await Task.Delay(-1);
    }

    private static Task Log(LogMessage log)
    {
        Console.WriteLine(log);
        return Task.CompletedTask;
    }

    private static Task Ready()
    {
        Console.WriteLine("Bot is connected and ready.");
        return Task.CompletedTask;
    }

    private static async Task MessageReceived(SocketMessage arg)
    {
        if (arg.Author.IsBot)
            return;

        Console.WriteLine($"Received: {arg.Content}");
        if (arg.Content.Contains("/ping"))
        {
            await arg.Channel.SendMessageAsync("Pong!");
            return;
        }

        _ = Task.Run(() => GenerateAndSendMessage(arg));
    }

    private static async Task GenerateAndSendMessage(SocketMessage arg)
    {
        //TODO вынести это в отдельную обработку, чтобы он мог продолжать слушать, даже когда генерирует ответ
        var answer = await _clientOllama.GetResponseAsync(arg.Content);

        Console.WriteLine($"Sending message: {answer}");
        await arg.Channel.SendMessageAsync(answer);
    }
}
