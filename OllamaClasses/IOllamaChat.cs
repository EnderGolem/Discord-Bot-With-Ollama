namespace DiscordBot.OllamaClasses;

internal interface IOllamaChat
{
    string Name { get; }
    bool AlwaysResponse { get; set; }

    Task<string?> GetResponseAsync(object requestContent);

    object CreateRequestContent(ulong channel);

    void AddToHistory(ulong channel, string author, string message);


}
