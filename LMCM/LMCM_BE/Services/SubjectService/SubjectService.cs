using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.DTOs.SubjectDtos;
using LMCM_BE.Models;
using LMCM_BE.Repositories.SubjectRepository.SubjectRepository;
using Microsoft.EntityFrameworkCore;

namespace LMCM_BE.Services.SubjectService
{
    public class SubjectService : ISubjectService
    {
        private readonly ISubjectRepository _subjectRepository;
        public SubjectService(ISubjectRepository subjectRepository)
        {
            _subjectRepository = subjectRepository;
        }

        public async Task<PagedResult<SubjectViewDto>> GetSubjectsAsync(string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            var data = await _subjectRepository.GetSubjectsAsync(searchKey, pageIndex, pageSize);
            return data;
        }

        public async Task<bool> InsertSubjectAsync(SubjectInsertDto subjectDto)
        {
            return await _subjectRepository.InsertSubjectAsync(subjectDto);
        }
        public async Task<bool> ImportSubjectsAsync(List<SubjectInsertDto> subjects)
        {
            return await _subjectRepository.ImportSubjectsAsync(subjects);
        }

        public async Task<Subject> GetSubjectByCodeAsync(string subjectCode)
        {
            return await _subjectRepository.GetSubjectByCodeAsync(subjectCode);
        }
        public async Task<List<Subject>> GetActiveSubjectsByCodesAsync(List<string> subjectCodes)
        {
            return await _subjectRepository.GetActiveSubjectsByCodesAsync(subjectCodes);
        }
    }
}
