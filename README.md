# Discord Bot With Ollama 
Discord-bot using the local ollama model to generate replies to messages. Fully customisable for your needs!

## 🌟 Features
- Integration with any Ollama model
- Contextual dialogue memory
- Flexible configuration system
- Timeout protection (20 sec)

## 📋 Requirements
- [.NET 6.0 SDK](https://dotnet.microsoft.com/download)
- [Ollama](https://ollama.com)
- Discord-bot with token


## 🛠️ Installation
```bash
# 1. Clone the repository
git clone https://github.com/yourrepo/discord-ollama-bot.git

# 2. Go to the project directory
cd discord-ollama-bot

# 3. Build the project
dotnet build
```

# ⚙️ Setup
## 1. Install the Ollama model
```bash
ollama pull llama3:70b # Example for Llama3-70B
```
## 2. Get Discord token:
- Create an app on the Discord Developer Portal
- Go to ‘Bot’ → ‘Add Bot’.
- Copy the token (you will need it for the config).

# 3. Set up the config:

### Option 1: Create a new config
1. Copy the config template:
```bash
copy Examples/appsettings.json appsettings.json
```
2. edit the copied config
```json
{
  "Discord": {
    "Token": "Insert Discord bot token here"
  },
  "Ollama": {
    "Name": "Insert here the Name of the model as it will be represented",
    "SystemPromt": "Insert here the system promt for the model",
    "Prompt": "Insert here the promt of the model, only needed for the generation model. The place where the message history will be inserted is #InnerPrompt. Required for Generate only",
    "LastMessageFormatting": "Insert here the last message formatting. The place where the last message will be inserted is #lastMessage.  Required for Generate only",
    "MessageMemorisedCount": "Insert here the number of messages the model will remember.",
    "ModelType": "Insert here the type of model Generate or Chat.",
    "ModelVersion": "Insert here the version of the Ollama model to be accessed",
    "OllamaURL": "Insert the URL to which the Ollama model will be accessed"
  }
}
```
### Option 2: Copy the finished config.
1. Select a suitable config from the Examples/ folder
2. Copy it to the root of the project
3. Substitute your Discord-Token





# 🚀 Using
```bash
dotnet run
```
## Available Commands
| | Command | Description |
|:------------------|:----------------------------------|
| `/ping` | testing the bot's functionality |
| `/alwaysRespond | | switching the response mode |

The bot will reply:
- To all messages (default)
- Only when mentioned (after using /alwaysRespond)

## ⚠️ Limitations
- Single threaded request processing
- Generation timeout: 20 seconds
- Recommended context size: 15 messages


