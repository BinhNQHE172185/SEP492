using LMCM_BE.Models;

namespace LMCM_BE.Repositories.SubjectRepository.SubjectRepository
{
    public interface ISubjectRepository
    {
        Task<Subject> GetSubjectAsync(string id);
    }
}
