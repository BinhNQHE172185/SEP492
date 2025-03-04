using LMCM_BE.Models;
using LMCM_BE.Repositories.SubjectRepository.SubjectRepository;

namespace LMCM_BE.Services.SubjectService
{
    public class SubjectService : ISubjectService
    {
        private readonly ISubjectRepository _subjectRepository;
        public SubjectService(ISubjectRepository subjectRepository)
        {
            _subjectRepository = subjectRepository;
        }

        public async Task<Subject> GetSubjectAsync(string id)
        {
            var data = await _subjectRepository.GetSubjectAsync(id);
            return data;
        }
    }
}
