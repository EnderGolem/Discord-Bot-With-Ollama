using Discord;
using Discord.WebSocket;
using System.Text;
using Newtonsoft.Json;

namespace DiscordBot;


class Program
{
    private static string _token = "MTMxNTc0MDkwMTQ2MDU0NTU1Ng.G4XWE3.NiaVNuVDRyONFxP5ve_92d8oNMPO12Rr3oLUf8";

    static async Task Main(string[] args)
    {
        OllamaClient clientOllama = new OllamaClient("http://localhost:11434/api/generate");
        DiscordClient discordClient = new DiscordClient(_token, clientOllama);

        await discordClient.Initialize();

        await Task.Delay(-1);
    }


}
