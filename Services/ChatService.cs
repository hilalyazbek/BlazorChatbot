using Azure;
using Azure.AI.OpenAI;

namespace BlazorChatbot.Services;

public class ChatService : IChatService
{
    public async Task<ChatCompletions> ChatWithOpenAI(string userMessage)
    {

        var _client = new OpenAIClient(
          new Uri("https://seesharpopenai.openai.azure.com/"),
          new AzureKeyCredential("[YOUR KEY HERE]"));

        Console.WriteLine(userMessage);
        Response<ChatCompletions> responseWithoutStream = await _client.GetChatCompletionsAsync(
            "seesharp",
            new ChatCompletionsOptions()
            {
                Messages =
                            {
                    new ChatMessage(ChatRole.User, userMessage)
                            },
                Temperature = (float)0.7,
                MaxTokens = 800,
                NucleusSamplingFactor = (float)0.95,
                FrequencyPenalty = 0,
                PresencePenalty = 0
            });

        return responseWithoutStream.Value;
    }
}
