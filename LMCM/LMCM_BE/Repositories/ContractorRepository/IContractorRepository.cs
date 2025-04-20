using LMCM_BE.Models;

namespace LMCM_BE.Repositories.ContractorRepository
{
    public interface IContractorRepository
    {
        Task<(List<Contractor>, int totalCount)> GetContractorsAsync(string? searchKey, int pageIndex = 1, int pageSize = 10);
        Task<List<Contractor>> GetContractorsListAsync(string? searchKey);
        Task<Contractor> GetContractorDetailByIdAsync(Guid contractorId);
        Task<Contractor?> GetActiveContractorByIdAsync(Guid contractorId);
        Task<Contractor?> GetContractorByIdAsync(Guid contractorId);
        Task<bool> CreateContractorAsync(Contractor contractor);
        Task<bool> UpdateContractorAsync(Contractor contractor);
        Task<Guid?> CheckContractor(string taxcode, string? email, string? phoneNumber);
    }
}
