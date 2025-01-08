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

        await Task.Run(() => { Console.WriteLine(132); });

        discordClient.Initialize();

        // Бот работает в фоновом режиме, пока не будет закрыт
        await Task.Delay(-1);
    }


}
