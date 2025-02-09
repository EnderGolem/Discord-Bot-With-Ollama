using Newtonsoft.Json;

namespace DiscordBot.Classes;

public class OllamaChatResponse
{
    [JsonProperty("model")]
    public string Model { get; set; } = string.Empty;

    [JsonProperty("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonProperty("message")]
    public ChatMessage Message { get; set; } = new ChatMessage();

    [JsonProperty("done_reason")]
    public string DoneReason { get; set; } = string.Empty;

    [JsonProperty("done")]
    public bool Done { get; set; }

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


    public class ChatMessage
    {
        [JsonProperty("role")]
        public string Role { get; set; } = string.Empty;

        [JsonProperty("content")]
        public string Content { get; set; } = string.Empty;
    }

}

