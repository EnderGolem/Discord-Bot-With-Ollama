namespace DiscordBot.Tests;

[TestClass]
public class OllamaClientGenerateTests
{
    [TestMethod]
    public void CreateRequestContent_ShouldIncludePromptAndUserMessage()
    {
        var url = "http://not_exist";
        var nameOfModel = "DefaultName";
        var systemPrompt = "Default system prompt";
        var prompt = "Default prompt #InnerPrompt";
        var model = "DefaultModel";
        int maxCountOfMessagesInHistory = 100;
        ulong channel = 111;

        var nameOfPerson = "Maria";
        var message = "Hello";

        var ollamaClientChat = new OllamaClientGenerate(
            apiUrl: url,
            name: nameOfModel,
            systemPrompt: systemPrompt,
            prompt: prompt,
            model: model,
            maxCountOfMessagesInHistory: maxCountOfMessagesInHistory
        );


        ollamaClientChat.AddToHistory(channel, nameOfPerson, message);

        var content = ollamaClientChat.CreateRequestContent(channel);
        var generatedPrompt = (string)content.GetType().GetProperty("prompt")!.GetValue(content)!;

        Assert.IsTrue(generatedPrompt.Contains(message));
        Assert.IsTrue(generatedPrompt.Contains(nameOfPerson));
    }

    [TestMethod]
    public void CreateRequestContent_WithFormattingLastMessage_ShouldIncludeFormattingAndMessage()
    {
        var url = "http://not_exist";
        var nameOfModel = "DefaultName";
        var systemPrompt = "Default system prompt";
        var prompt = "Default prompt #InnerPrompt";
        var formattingMessage = "Last Message";
        var formattingMessageWithLasMessage = $"{formattingMessage}: #lastMessage";
        var model = "DefaultModel";
        int maxCountOfMessagesInHistory = 100;
        ulong channel = 111;

        var nameOfPerson = "Maria";
        var message = "Hello";

        var ollamaClientChat = new OllamaClientGenerate(
            apiUrl: url,
            name: nameOfModel,
            systemPrompt: systemPrompt,
            prompt: prompt,
            model: model,
            maxCountOfMessagesInHistory: maxCountOfMessagesInHistory,
            lastMessageFormatting: formattingMessageWithLasMessage
        );


        ollamaClientChat.AddToHistory(channel, nameOfPerson, message);

        var content = ollamaClientChat.CreateRequestContent(channel);
        var generatedPrompt = (string)content.GetType().GetProperty("prompt")!.GetValue(content)!;

        Assert.IsTrue(generatedPrompt.Contains(message));
        Assert.IsTrue(generatedPrompt.Contains(formattingMessage));
    }

    [TestMethod]
    public void CreateRequestContent_WithIncorrectFormattingLastMessage_ShouldIncludeFormattingAndNotMessage()
    {
        var url = "http://not_exist";
        var nameOfModel = "DefaultName";
        var systemPrompt = "Default system prompt";
        var prompt = "Default prompt #InnerPrompt";
        var formattingMessage = "Last Message";
        var formattingMessageWithLasMessage = $"{formattingMessage}";
        var model = "DefaultModel";
        int maxCountOfMessagesInHistory = 100;
        ulong channel = 111;

        var nameOfPerson = "Maria";
        var message = "Hello";

        var ollamaClientChat = new OllamaClientGenerate(
            apiUrl: url,
            name: nameOfModel,
            systemPrompt: systemPrompt,
            prompt: prompt,
            model: model,
            maxCountOfMessagesInHistory: maxCountOfMessagesInHistory,
            lastMessageFormatting: formattingMessageWithLasMessage
        );


        ollamaClientChat.AddToHistory(channel, nameOfPerson, message);

        var content = ollamaClientChat.CreateRequestContent(channel);
        var generatedPrompt = (string)content.GetType().GetProperty("prompt")!.GetValue(content)!;

        Assert.IsFalse(generatedPrompt.Contains(message));
        Assert.IsTrue(generatedPrompt.Contains(formattingMessage));
    }
}
