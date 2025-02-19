using DiscordBot.Classes;
using DiscordBot.OllamaClasses;
using Newtonsoft.Json;

namespace DiscordBot.Tests;

[TestClass]
public class OllamaClientChatTests
{
    [TestMethod]
    public void CreateRequestContent_ShouldIncludeSystemPromptAndUserMessage()
    {
        var url = "http://not_exist";
        var nameOfModel = "DefaultName";
        var systemPrompt = "Default system prompt";
        var prompt = "Default prompt";
        var model = "DefaultModel";
        int maxCountOfMessagesInHistory = 100;
        ulong channel = 111;

        var nameOfPerson = "Maria";
        var message = "Hello";

        var ollamaClientChat = new OllamaClientChat(
            apiUrl: url,
            name: nameOfModel,
            systemPrompt: systemPrompt,
            prompt: prompt,
            model: model,
            maxCountOfMessagesInHistory: maxCountOfMessagesInHistory
        );


        ollamaClientChat.AddToHistory(channel, nameOfPerson, message);

        var content =  ollamaClientChat.CreateRequestContent(channel);

        var responseObject = JsonConvert.SerializeObject(content);
        
        Assert.IsTrue(responseObject.Contains($"\"role\":\"system\",\"content\":\"{systemPrompt}\""));
        Assert.IsTrue(responseObject.Contains("\"role\":\"user\""));
        Assert.IsTrue(responseObject.Contains(message));
        Assert.IsTrue(responseObject.Contains(nameOfPerson));
    }

    [TestMethod]
    public void AddToHistory_OneTime_ShouldLengthTwo()
    {
        var url = "http://not_exist";
        var nameOfModel = "DefaultName";
        var systemPrompt = "Default system prompt";
        var prompt = "Default prompt";
        var model = "DefaultModel";
        int maxCountOfMessagesInHistory = 100;
        ulong channel = 111;

        var nameOfPerson = "Maria";
        var message = "Hello";

        var ollamaClientChat = new OllamaClientChat(
            apiUrl: url,
            name: nameOfModel,
            systemPrompt: systemPrompt,
            prompt: prompt,
            model: model,
            maxCountOfMessagesInHistory: maxCountOfMessagesInHistory
        );


        ollamaClientChat.AddToHistory(channel, nameOfPerson, message);

        var content = ollamaClientChat.CreateRequestContent(channel);
        var length = ((dynamic[])content.GetType().GetProperty("messages")!.GetValue(content)!).Length;

        Assert.AreEqual(1 + 1, length);
    }

    [TestMethod]
    public void AddToHistory_MoreThanSizeOfMemory_ShouldReturnSizeOfMemory()
    {
        var url = "http://not_exist";
        var nameOfModel = "DefaultName";
        var systemPrompt = "Default system prompt";
        var prompt = "Default prompt";
        var model = "DefaultModel";
        int maxCountOfMessagesInHistory = 2;
        ulong channel = 111;

        var nameOfPerson = "Maria";
        var message = "Hello";

        var ollamaClientChat = new OllamaClientChat(
            apiUrl: url,
            name: nameOfModel,
            systemPrompt: systemPrompt,
            prompt: prompt,
            model: model,
            maxCountOfMessagesInHistory: maxCountOfMessagesInHistory
        );


        ollamaClientChat.AddToHistory(channel, nameOfPerson, message);
        ollamaClientChat.AddToHistory(channel, nameOfPerson, message);
        ollamaClientChat.AddToHistory(channel, nameOfPerson, message);
        ollamaClientChat.AddToHistory(channel, nameOfPerson, message);

        var content = ollamaClientChat.CreateRequestContent(channel);
        var length = ((dynamic[])content.GetType().GetProperty("messages")!.GetValue(content)!).Length;

        Assert.AreEqual(maxCountOfMessagesInHistory + 1, length);
    }

}