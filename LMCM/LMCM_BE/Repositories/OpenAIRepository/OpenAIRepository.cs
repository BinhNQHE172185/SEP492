using LMCM_BE.Repositories.OpenAIRepository;
using OpenAI;
using OpenAI.Assistants;
using OpenAI.Files;
using static LMCM_BE.DTOs.OpenAIDtos.OpenAIDto;

public class OpenAIRepository : IOpenAIRepository
{
    private readonly OpenAIClient _client;
    private readonly string _assistantId;

    public OpenAIRepository(IConfiguration configuration)
    {
        var apiKey = configuration["OpenAI:ApiKey"];
        _assistantId = configuration["OpenAI:AssistantId"];
        _client = new OpenAIClient(apiKey);
    }

    public async Task<string?> UploadAndAnalyzeFileAsync(Stream file, string fileName, string prompt)
    {
        try
        {
            var fileClient = _client.GetOpenAIFileClient();
            var assistantClient = _client.GetAssistantClient();

            // 1. Upload file
            var uploadFile = await fileClient.UploadFileAsync(file, fileName, FileUploadPurpose.Assistants);

            // 3. Create a thread
            var thread = await assistantClient.CreateThreadAsync();

            // 4. Gửi message với file đính kèm
            var message = await assistantClient.CreateMessageAsync(
                thread.Value.Id,
                MessageRole.User,
            new List<MessageContent>
            {
                MessageContent.FromText(prompt)
            },
            new MessageCreationOptions
            {
                Attachments = {
                    new MessageCreationAttachment(uploadFile.Value.Id, new List<ToolDefinition> { ToolDefinition.CreateFileSearch() })
                }
            });
            //5. Run
            string finalOutput = string.Empty;

            await foreach (var update in assistantClient.CreateRunStreamingAsync(thread.Value.Id, _assistantId, new RunCreationOptions()))
            {
                if (update is MessageContentUpdate contentUpdate)
                {
                    var text = contentUpdate.Text;
                    finalOutput += text;
                }
            }

            return string.IsNullOrWhiteSpace(finalOutput) ? "No content returned." : finalOutput;
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }
}
