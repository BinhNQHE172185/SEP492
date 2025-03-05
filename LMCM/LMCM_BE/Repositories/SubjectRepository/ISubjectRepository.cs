using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.DTOs.SubjectDtos;
using LMCM_BE.Models;

namespace LMCM_BE.Repositories.SubjectRepository.SubjectRepository
{
    public interface ISubjectRepository
    {
        Task<PagedResult<SubjectViewDto>> GetSubjectsAsync(string? searchKey, int pageIndex = 1, int pageSize = 10);
    }
}
