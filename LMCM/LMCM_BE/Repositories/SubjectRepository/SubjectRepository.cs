using LMCM_BE.DbContext;
using LMCM_BE.Models;
using Microsoft.EntityFrameworkCore;

namespace LMCM_BE.Repositories.SubjectRepository.SubjectRepository
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly LMCM_DBContext _dbContext;
        public SubjectRepository(LMCM_DBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Subject> GetSubjectAsync(string id)
        {
            var data = await _dbContext.Subjects
                                .Where(u => u.SubjectCode.Trim().ToLower() == id.Trim().ToLower())
                                .FirstOrDefaultAsync();
            return data;
        }
    }
}
