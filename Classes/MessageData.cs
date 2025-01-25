using Discord.WebSocket;
using Discord;

namespace DiscordBot.Classes;

public record MessageData(
    DateTime Timestamp,
    string Content,
    ISocketMessageChannel Channel,
    MessageReference Reference
);
