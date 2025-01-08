using Newtonsoft.Json;

namespace DiscordBot.Classes;

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
