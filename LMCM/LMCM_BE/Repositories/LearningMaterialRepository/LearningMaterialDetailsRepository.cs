using AutoMapper;
using LMCM_BE.DbContext;
using LMCM_BE.DTOs.LearningMaterialDtos;
using LMCM_BE.Models;

namespace LMCM_BE.Repositories.LearningMaterialRepository
{
    public class LearningMaterialDetailsRepository : ILearningMaterialDetailsRepository
    {
        private readonly LMCM_DBContext _dbContext;
        private readonly IMapper _mapper;
        public LearningMaterialDetailsRepository(LMCM_DBContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public async Task<LearningMaterialDetail> InsertMaterialDetailsAsync(LearningMaterialDetailsInsertDto detail)
        {
            if (detail == null )
                throw new ArgumentNullException(nameof(detail));

            var newDetail = new LearningMaterialDetail
            {
                MaterialDetailId = Guid.NewGuid(),
                MaterialName = detail.MaterialName,
                MaterialDescription = detail.MaterialDescription,
                Isbn = detail.Isbn,
                Type = detail.Type,
                Note = detail.Note,
                Author = detail.Author,
                Publisher = detail.Publisher,
                PublishedDate = detail.PublishedDate,
                Edition = detail.Edition,
                Url = detail.Url,
                Status = "Active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _dbContext.LearningMaterialDetails.AddAsync(newDetail);
            return newDetail;
        }
    }
}
