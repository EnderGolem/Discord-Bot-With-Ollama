namespace DiscordBot.OllamaClasses;

internal interface IOllamaClient
{
    string Name { get; }

    Task<string?> GetResponseAsync(object requestContent);

    object CreateRequestContent(ulong channel);

    void AddToHistory(ulong channel, string author, string message);


}
