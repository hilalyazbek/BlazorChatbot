using Azure.AI.OpenAI;

namespace BlazorChatbot.Services;

public interface IChatService
{
    Task<ChatCompletions> ChatWithOpenAI(string userMessage);
}
