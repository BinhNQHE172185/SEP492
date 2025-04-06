using LMCM_BE.DTOs.CLODtos;
using LMCM_BE.DTOs.ConstructivistQuestionDtos;
using LMCM_BE.DTOs.GradingStructureDtos;
using LMCM_BE.DTOs.LearningMaterialDtos;
using LMCM_BE.DTOs.ScheduleDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.DTOs.SubjectDtos;
using LMCM_BE.DTOs.SyllabusDtos;
using LMCM_BE.Models;
using LMCM_BE.Repositories.SubjectRepository.SubjectRepository;
using LMCM_BE.Repositories.SyllabusRepository;

namespace LMCM_BE.Services.SyllabusService
{
    public class SyllabusService : ISyllabusService
    {
        private readonly ISyllabusRepository _syllabusRepository;
        public SyllabusService(ISyllabusRepository syllabusRepository)
        {
            _syllabusRepository = syllabusRepository;
        }

        public async Task<bool> DeleteSyllabusAsync(Guid id)
        {
            return await _syllabusRepository.DeleteSyllabusAsync(id);
        }

        public async Task<PagedResult<SyllabusListViewDto>> GetSyllabusesAsync(string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            var data = await _syllabusRepository.GetSyllabusesAsync(searchKey, pageIndex, pageSize);
            return data;
        }
        public async Task<PagedResult<SyllabusListViewDto>> GetSyllabusChangeHistoriesAsync(Guid? syllabusId, string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            var data = await _syllabusRepository.GetSyllabusChangeHistoriesAsync(syllabusId, searchKey, pageIndex, pageSize);
            return data;
        }
        public async Task<Syllabus> ImportSyllabusAsync(SyllabusInsertDto syllabus, List<ScheduleInsertDto> schedules,
            List<CLOInsertDto> cLOs, List<GradingStructureInsertDto> gradingStructures,
            List<ConstructivistQuestionInsertDto>? constructivistQuestions,
            List<LearningMaterialImportDto>? learningMaterials, bool keepUserCreated)
        {
            return await _syllabusRepository.ImportSyllabusAsync(syllabus,schedules,cLOs,gradingStructures,constructivistQuestions,learningMaterials,keepUserCreated);
        }
        public async Task<Syllabus?> GetActiveSyllabusBySubjectIdAsync(Guid subjectId)
        {
            return await _syllabusRepository.GetActiveSyllabusBySubjectIdAsync(subjectId);
        }

        public async Task<SyllabusDetailDto> GetSyllabusDetailAsync(Guid? syllabusId)
        {
            return await _syllabusRepository.GetSyllabusDetailAsync(syllabusId);
        }

        public async Task<Syllabus> GetSyllabusByIdAsync(Guid? syllabusId)
        {
            return await _syllabusRepository.GetSyllabusByIdAsync(syllabusId);
        }
    }
}
