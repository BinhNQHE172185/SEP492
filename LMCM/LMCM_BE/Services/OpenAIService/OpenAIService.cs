using LMCM_BE.Services.ContractorService;
using System.Text.Json;
using static LMCM_BE.DTOs.OpenAIDtos.OpenAIDto;
using LMCM_BE.DTOs.ContractorDtos;
using OpenAI;
using OpenAI.Assistants;
using OpenAI.Files;
using LMCM_BE.Services.ContractService;

namespace LMCM_BE.Services.OpenAIService
{
    public class OpenAIService : IOpenAIService
    {
        private readonly IContractorService _contractorService;
        private readonly IContractService _contractService;
        private readonly OpenAIClient _client;
        private readonly string _contractAssistantId;
        private readonly string _recordAssistantId;

        public OpenAIService(IContractorService contractorService, IContractService contractService, IConfiguration configuration)
        {
            _contractorService = contractorService;
            _contractService = contractService;
            var apiKey = configuration["OpenAI:ApiKey"];
            _contractAssistantId = configuration["OpenAI:ContractAssistantId"];
            _recordAssistantId = configuration["OpenAI:RecordAssistantId"];
            _client = new OpenAIClient(apiKey);
        }

        public async Task<string?> UploadAndAnalyzeFileAsync(Stream file, string fileName, string prompt, string assisstantId)
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

                await foreach (var update in assistantClient.CreateRunStreamingAsync(thread.Value.Id, assisstantId, new RunCreationOptions()))
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

        public async Task<ContractInfoFromAIDto?> UploadAndAnalyzeContractFileAsync(Stream fileStream, string fileName, string prompt)
        {
            var result = await UploadAndAnalyzeFileAsync(fileStream, fileName, prompt, _contractAssistantId);
            var aiContractorInfo = JsonSerializer.Deserialize<ContractInfoFromAIDto>(result);

            if (aiContractorInfo == null)
                return null;

            var existingId = await _contractorService.CheckContractor(
                aiContractorInfo.taxCode,
                aiContractorInfo.email,
                aiContractorInfo.phoneNumber);

            if (existingId != null)
            {
                aiContractorInfo.contractorId = existingId.ToString();
                return aiContractorInfo;
            }

            if (!string.IsNullOrWhiteSpace(aiContractorInfo.contractorName))
            {
                var newContractor = new ContractorCreateDto
                {
                    ContractorName = aiContractorInfo.contractorName,
                    Address = aiContractorInfo.address,
                    PhoneNumber = aiContractorInfo.phoneNumber,
                    TaxCode = aiContractorInfo.taxCode,
                    Email = aiContractorInfo.email,
                    EmployeeCode = aiContractorInfo.employeeCode,
                    IdCardNumber = aiContractorInfo.idCardNumber,
                    IdIssuedPlace = aiContractorInfo.idIssuedPlace,
                    Position = aiContractorInfo.position,
                    BankAccountNumber = aiContractorInfo.bankAccountNumber,
                    BankName = aiContractorInfo.bankName
                };

                var isCreated = await _contractorService.CreateContractorAsync(newContractor);
                if (isCreated)
                {
                    var id = await _contractorService.CheckContractor(
                       aiContractorInfo.taxCode,
                       aiContractorInfo.email,
                       aiContractorInfo.phoneNumber);
                    aiContractorInfo.contractorId = id.ToString();
                }
            }

            return aiContractorInfo;
        }
        public async Task<AcceptantRecordInfoFromAIDto?> UploadAndAnalyzeRecordFileAsync(Stream fileStream, string fileName, string prompt)
        {
            var result = await UploadAndAnalyzeFileAsync(fileStream, fileName, prompt, _recordAssistantId);
            var aAiReocrdInfo = JsonSerializer.Deserialize<AcceptantRecordInfoFromAIDto>(result);

            if (aAiReocrdInfo == null)
                return null;

            var existingId = await _contractService.CheckContractByTitle(aAiReocrdInfo.contractTitle);

            if (existingId != null)
            {
                aAiReocrdInfo.contractId = existingId.ToString();
                return aAiReocrdInfo;
            }

            return aAiReocrdInfo;
        }
    }
}
