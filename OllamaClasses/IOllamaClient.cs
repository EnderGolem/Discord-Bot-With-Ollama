namespace DiscordBot.OllamaClasses;

internal interface IOllamaClient
{
    string Name { get; }
    bool AlwaysResponse { get; set; }

    Task<string?> GetResponseAsync(object requestContent);

    object CreateRequestContent(ulong channel);

    void AddToHistory(ulong channel, string author, string message);


}
