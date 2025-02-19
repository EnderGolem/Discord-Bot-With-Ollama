using Discord;
using Discord.WebSocket;

namespace DiscordBot.Classes;

public record MessageData(
    DateTime Timestamp,
    string Content,
    ISocketMessageChannel Channel,
    MessageReference Reference
);
