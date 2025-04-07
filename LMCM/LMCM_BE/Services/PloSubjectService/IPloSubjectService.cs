namespace LMCM_BE.Services.PloSubjectService
{
    public interface IPloSubjectService
    {
        Task<bool> DeletePloSubjectsAsync(Guid curriculumId);
    }
}
