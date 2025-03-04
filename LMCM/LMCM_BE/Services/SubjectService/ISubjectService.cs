using LMCM_BE.Models;

namespace LMCM_BE.Services.SubjectService
{
    public interface ISubjectService
    {
        Task<Subject> GetSubjectAsync(string id);
    }
}
