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
        string name = "Лами";
        string prompt = $"Ты {name} чат бот." +
                        $"1) Учитывай историю чтобы сохранить контекст " +
                        $"2) Веди себя как пользователь дискорд серверов" +
                        $"3) Пиши коротко и дружелюбно, используя 1–2 предложения " +
                        $"4) Отвечай только на русском языке. " +
                            $"Вот история переписки: {"#InnerPrompt"}" +
                            $"Ответь на последнее сообщение, как {name}.";

        OllamaClient clientOllama = new OllamaClient("http://localhost:11434/api/generate", "Лами", prompt);
        DiscordClient discordClient = new DiscordClient(_token, clientOllama);

        await discordClient.Initialize();

        await Task.Delay(-1);
    }


}
