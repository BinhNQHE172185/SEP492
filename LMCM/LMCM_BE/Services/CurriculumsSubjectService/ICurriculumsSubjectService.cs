namespace LMCM_BE.Services.CurriculumsSubjectService
{
    public interface ICurriculumsSubjectService
    {
        Task<bool> DeleteCurriculumsSubjectAsync(Guid curriculumId);
    }
}
