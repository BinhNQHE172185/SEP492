using LMCM_BE.DTOs.CLODtos;
using LMCM_BE.DTOs.ConstructivistQuestionDtos;
using LMCM_BE.DTOs.GradingStructureDtos;
using LMCM_BE.DTOs.LearningMaterialDtos;
using LMCM_BE.DTOs.ScheduleDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.DTOs.SubjectDtos;
using LMCM_BE.DTOs.SyllabusDtos;
using LMCM_BE.Models;

namespace LMCM_BE.Repositories.SyllabusRepository
{
    public interface ISyllabusRepository
    {
        Task<PagedResult<SyllabusListViewDto>> GetSyllabusesAsync(string? searchKey, int pageIndex = 1, int pageSize = 10);
        Task<PagedResult<SyllabusListViewDto>> GetSyllabusChangeHistoriesAsync(Guid? syllabusId,string? searchKey, int pageIndex = 1, int pageSize = 10);
        Task<Syllabus> ImportSyllabusAsync(SyllabusInsertDto syllabus,List<ScheduleInsertDto> schedules,
            List<CLOInsertDto> cLOs, List<GradingStructureInsertDto> gradingStructures,
            List<ConstructivistQuestionInsertDto>? constructivistQuestions,
            List<LearningMaterialImportDto>? learningMaterials, bool keepUserCreated);
        Task<SyllabusDetailDto> GetSyllabusDetailAsync(Guid? syllabusId);
        Task<Syllabus> GetSyllabusByIdAsync(Guid? syllabusId);
        Task<bool> UpdateSyllabusAsync(Syllabus existingSyllabus, SyllabusInsertDto syllabusDto);
        Task<bool> DeleteSyllabusAsync(Guid id);
        Task<Syllabus?> GetActiveSyllabusBySubjectIdAsync(Guid subjectId);
    }
}
