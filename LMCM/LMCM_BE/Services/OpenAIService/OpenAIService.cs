using LMCM_BE.Repositories.OpenAIRepository;
using LMCM_BE.Services.ContractorService;
using System.Text.Json;
using static LMCM_BE.DTOs.OpenAIDtos.OpenAIDto;
using LMCM_BE.DTOs.ContractorDtos;

namespace LMCM_BE.Services.OpenAIService
{
    public class OpenAIService : IOpenAIService
    {
        private readonly IOpenAIRepository _repository;
        private readonly IContractorService _contractorService;

        public OpenAIService(IOpenAIRepository repository, IContractorService contractorService)
        {
            _repository = repository;
            _contractorService = contractorService;
        }

        public async Task<ContractorInfoFromAIDto?> UploadAndAnalyzeFileAsync(Stream fileStream, string fileName, string prompt)
        {
            var result = await _repository.UploadAndAnalyzeFileAsync(fileStream, fileName, prompt);
            var aiContractorInfo = JsonSerializer.Deserialize<ContractorInfoFromAIDto>(result);

            if (aiContractorInfo == null)
                return null;

            var existingId = await _contractorService.CheckContractor(
                aiContractorInfo.taxCode,
                aiContractorInfo.email,
                aiContractorInfo.phoneNumber);

            if (existingId != null)
            {
                aiContractorInfo.contractorId = existingId;
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
                    aiContractorInfo.contractorId = await _contractorService.CheckContractor(
                        aiContractorInfo.taxCode,
                        aiContractorInfo.email,
                        aiContractorInfo.phoneNumber);
                }
            }

            return aiContractorInfo;
        }
    }
}
