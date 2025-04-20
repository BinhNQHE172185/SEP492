using Microsoft.AspNetCore.Mvc;

namespace LMCM_BE.DTOs.OpenAIDtos
{
    public class OpenAIDto
    {
        public class AnalyzeContractRequest
        {
            [FromForm(Name = "file")]
            public IFormFile File { get; set; }

            [FromForm(Name = "prompt")]
            public string Prompt { get; set; }
        }
        public class PromptRequest
        {
            public string Prompt { get; set; }
            public string Instruction { get; set; }
            public string Name { get; set; }
        }
        public class ContractorInfoFromAIDto
        {
            public string? proposalId { get; set; }
            public Guid? contractorId { get; set; }
            public string? title { get; set; }
            public decimal? contractValue { get; set; }
            public DateTime? startDate { get; set; }
            public DateTime? endDate { get; set; }
            public string? contractorName { get; set; }
            public string? address { get; set; }
            public string? phoneNumber { get; set; }
            public string? taxCode { get; set; }
            public string? email { get; set; }
            public string? employeeCode { get; set; }
            public string? idCardNumber { get; set; }
            public string? idIssuedPlace { get; set; }
            public string? position { get; set; }
            public string? bankAccountNumber { get; set; }
            public string? bankName { get; set; }
        }
    }
}
