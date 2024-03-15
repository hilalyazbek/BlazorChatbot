# Creating an OpenAI Chatbot using your own data - Part 2 - Creating a Blazor Chatbot

# Prerequisites

- [Azure Subscription](https://azure.microsoft.com/en-us/free/) (Limited Free Credit)
- Open AI Resource ([Check out part 1 of this series](https://www.hilalyazbek.com/blogposts/creating-an-openai-chatbot-using-your-data-part-1))
- IDE ([Visual Studio 2022 Community](https://visualstudio.microsoft.com/vs/community/))
- Basic .NET Development knowledge

# Creating the Blazor Application

Open your favorite IDE and create a Blazor Web Application. I am using Visual Studio 2022.

If you are new to Blazor, Check out these blog posts.

> [From .NET to the Web: Building a Blazor Front-End](https://www.hilalyazbek.com/blogposts/from-net-to-the-web-building-a-blazor-front-end)
> 

> [From .NET to the Web: Building a Blazor Front-End Part 2 - Full CRUD Application](https://www.hilalyazbek.com/blogposts/from-net-to-the-web-building-a-blazor-front-end-part-2-full-crud-application)
> 

> **[Blazor Server App Authentication - Simplifying Authentication in .NET Core API with JWT - Part II](https://www.hilalyazbek.com/blogposts/blazor-server-app-authentication-simplifying-authentication-in-net-core-api)**
> 

# Creating the Chat Service

Open the terminal and add the below package. Note: The package is still in prerelease

```csharp
dotnet add package Azure.AI.OpenAI --version 1.0.0-beta.5
```

Next, Create a chat service interface and implementation. Keep it simple.

I created the below Interface

```csharp
public interface IChatService
{
    Task<ChatCompletions> ChatWithOpenAI(string userMessage);
}
```

This interface only has one method, “**ChatWithOpenAI**”. It takes the user message as input and returns a **ChatCompletions** object (a partial class in the **Azure.AI.OpenAI** package I installed above)

```csharp
public partial class ChatCompletions
{
    internal ChatCompletions(string id, int? created, IReadOnlyList<ChatChoice> choices, CompletionsUsage usage)
    {
        Id = id;
        Created = TimeConverters.DateTimeFromUnixEpoch(created.Value);
        Choices = choices.ToList();
        Usage = usage;
    }

    /// <summary>
    /// Gets a unique identifier associated with a chat completions response.
    /// </summary>
    public string Id { get; }
    /// <summary>
    /// Gets the UTC timestamp at which this chat completions response was generated.
    /// </summary>
    public DateTime Created { get; }
    /// <summary>
    /// Gets the collection of chat choices generated in a chat completions request.
    /// </summary>
    public IReadOnlyList<ChatChoice> Choices { get; }
    /// <summary>
    /// Gets usage counts for tokens using the chat completions API.
    /// </summary>
    public CompletionsUsage Usage { get; }
}
```

Implementing the IChatService interface is very straightforward. Create a class called ChatService, and paste the below code.

```csharp
public class ChatService : IChatService
{
    public async Task<ChatCompletions> ChatWithOpenAI(string userMessage)
    {

        var _client = new OpenAIClient(
          new Uri("https://seesharpopenai.openai.azure.com/"),
          new AzureKeyCredential([YOUR KEY HERE]));

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
```

This method creates a new OpenAIClient with a URI and AzureKeyCredential that you can copy from your AzureOpenAI resource.

Next, the GetChatCompletionsAsync method is called with the user message and other properties.

- Temperature (0.0 - 2.0) :  used to set the accuracy and creativity of the model. The lower the value the more focused and deterministic the reply is.
- MaxTokens: The maximum tokens to be generated.

For the full documentation, visit the below link

> [https://www.nuget.org/packages/Azure.AI.OpenAI](https://www.nuget.org/packages/Azure.AI.OpenAI)
> 

Open the Program.cs file and paste the below service registration in the container

```csharp
builder.Services.AddScoped<IChatService, ChatService>();
```

Now that the method is complete, I will setup a simple Razor Page with a few components to create the UI.

**Warning: It’s not going to be pretty** 😃

# Creating the UI

You can create a new page, or use a page from the boiler plate code.

I will need three components:

- A text area to view the chat and the chat history.
- An input field to allow the user to ask the questions.
- A “Send” button to trigger the event.

```csharp
<h1>Open AI Chatbot</h1>

<div>
    <textarea disabled="disabled" class="chatboxresult" @bind="_conversationHistory"></textarea>
</div>
<div>
    <input @bind="_inputMessage" placeholder="Chat with me" class="chatboxinput" />
    <button class="btn btn-primary" @onclick="SendChat">Click me</button>
</div>
```

Next, I need to inject the IChatService into the Razor page.

```csharp
@page "/"
@rendermode InteractiveServer
@using BlazorChatbot.Services // Using the Services namespace
@using System.Text
@inject IChatService IChatService // Injecting the Chat Service
```

In the “Code” section of the page, paste the below

```csharp
@code
{
    private string? _conversationHistory = string.Empty;
    private string? _inputMessage = string.Empty;
    private StringBuilder? _conversation = new StringBuilder();

    private async Task SendChat()
    {
        var completions = await IChatService.ChatWithOpenAI(_inputMessage);

        _conversation.AppendLine("Question: " + _inputMessage);

        var answer = completions.Choices[0].Message.Content;

        _conversation.AppendLine("Answer: " + answer);
        _conversation.AppendLine();

        _conversationHistory = _conversation.ToString();

        _inputMessage = string.Empty;

    }
}
```

The code is very simple. 

- I am calling the ChatWithOpenAI method in the ChatService that we just created.
- Getting the answer.
- Appending the answer to the _conversation string builder object.
- Resetting the _inputmessage to empty (to clear the previous user question).

# SeeSharp

There you have it, I just created a simple Blazor application that connects with OpenAI that I can chat with.

In Part 3 and the final part, I will be able to chat with my own dataset that I uploaded to Azure OpenAI.

If you like this content, consider following me on [Medium](https://medium.com/@hilalyazbek), and [LinkedIn](https://www.linkedin.com/in/hilalyazbek)
