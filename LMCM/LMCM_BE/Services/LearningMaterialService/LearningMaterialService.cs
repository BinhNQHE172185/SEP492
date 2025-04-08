using AutoMapper;
using LMCM_BE.DTOs.LearningMaterialDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Models;
using LMCM_BE.Repositories.LearningMaterialRepository;
using LMCM_BE.Repositories.SyllabusRepository;
using LMCM_BE.Shared.Constant;
using LMCM_BE.UnitOfWork;

namespace LMCM_BE.Services.LearningMaterialService
{
    public class LearningMaterialService : ILearningMaterialService
    {
        private readonly ILearningMaterialRepository _materialRepository;
        private readonly ISyllabusRepository _syllabusRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public LearningMaterialService(ILearningMaterialRepository materialRepository, ISyllabusRepository syllabusRepository, IMapper mapper,IUnitOfWork unitOfWork)
        {
            _materialRepository = materialRepository;
            _syllabusRepository = syllabusRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> DeleteLearningMaterialByIdAsync(Guid materialId)
        {
            if (materialId == Guid.Empty)
                throw new ArgumentException("Id bị trống.", nameof(materialId));
            var learningMaterial = await _materialRepository.GetLearningMaterialByIdAsync(materialId);

            if (learningMaterial == null)
                throw new KeyNotFoundException("Không tìm thấy dữ liệu.");
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                learningMaterial.Status = LearningMaterialStatus.Deleted;
                learningMaterial.UpdatedAt = DateTime.UtcNow;
                await _materialRepository.UpdateLearningMaterialAsync(learningMaterial);
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }

        public async Task<LearningMaterialViewDto> GetLearningMaterialByIdAsync(Guid materialId)
        {
            if (materialId == Guid.Empty)
                throw new ArgumentException("Id bị trống.", nameof(materialId));
            var learningMaterial= await _materialRepository.GetLearningMaterialByIdAsync(materialId);

            if (learningMaterial == null)
                throw new KeyNotFoundException("Không tìm thấy dữ liệu.");

            return _mapper.Map<LearningMaterialViewDto>(learningMaterial);
        }

        public async Task<PagedResult<LearningMaterialListDto>> GetMaterialsBySyllabusIdAsync(Guid syllabusId,string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            var (items,totalCount)= await _materialRepository.GetMaterialsBySyllabusIdAsync(syllabusId, searchKey, pageIndex, pageSize);
            var data = _mapper.Map<List<LearningMaterialListDto>>(items);

            return new PagedResult<LearningMaterialListDto>
            {
                Items = data,
                TotalCount = totalCount,
                CurrentPage = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<List<LearningMaterialListDto>> GetMaterialsBySyllabusIdAsync(Guid syllabusId)
        {
            var items= await _materialRepository.GetMaterialsBySyllabusIdAsync(syllabusId);
            var data = _mapper.Map<List<LearningMaterialListDto>>(items);
            return data;
        }

        public async Task<bool> InsertLearningMaterialAsync(LearningMaterialInsertDto material)
        {
            if (material == null)
                throw new ArgumentNullException(nameof(material));
            var syllabus = await _syllabusRepository.GetSyllabusByIdAsync(material.SyllabusId);
            if (syllabus == null)
                throw new KeyNotFoundException($"Không tìm thấy đề cương với ID: {material.SyllabusId}");
            var newMaterial = _mapper.Map<LearningMaterial>(material);
            newMaterial.MaterialId = Guid.NewGuid();
            newMaterial.IsImportedMaterial = false;
            newMaterial.Status = LearningMaterialStatus.Active;
            newMaterial.CreatedAt = DateTime.UtcNow;
            newMaterial.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                await _materialRepository.InsertLearningMaterialAsync(newMaterial);
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }
        public async Task<bool> ImportLearningMaterialsAsync(List<LearningMaterial> materials, Guid? oldSyllabusId, Guid newSyllabusId, bool keepUserCreated)
        {
            if (materials == null || !materials.Any())
                return false;

            foreach (var material in materials)
            {
                material.SyllabusId = newSyllabusId;
                material.MaterialId = Guid.NewGuid();
                material.IsMainMaterial = false;
                material.IsImportedMaterial = true;
                material.MaterialType = MaterialType.HocLieu;
                material.Status = LearningMaterialStatus.Active;
                material.CreatedAt = DateTime.UtcNow;
                material.UpdatedAt = DateTime.UtcNow;
            }

            if (oldSyllabusId != null && keepUserCreated)
            {
                var existingMaterials = await _materialRepository.GetUserCreatedMaterialsFromSyllabusIdAsync(oldSyllabusId.Value);
                foreach (var material in existingMaterials)
                {
                    var newMaterial = new LearningMaterial
                    {
                        MaterialId = Guid.NewGuid(),  // Create a new MaterialId
                        SyllabusId = newSyllabusId,   // Assign the new syllabus
                        LearningType = material.LearningType,
                        MaterialType = material.MaterialType,
                        IsMainMaterial = material.IsMainMaterial,
                        IsImportedMaterial = false,    // Mark as imported material
                        MaterialName = material.MaterialName,
                        Isbn = material.Isbn,
                        Author = material.Author,
                        Publisher = material.Publisher,
                        PublishedDate = material.PublishedDate,
                        Edition = material.Edition,
                        Url = material.Url,
                        Purpose = material.Purpose,
                        Note = material.Note,
                        Status = LearningMaterialStatus.Active,
                        CreatedAt = DateTime.UtcNow,  // Set created date to current time
                        UpdatedAt = DateTime.UtcNow  // Set updated date to current time
                    };

                    // Add the new material copy to the new materials list
                    materials.Add(newMaterial);
                }
            }

            await _materialRepository.AddMaterialsAsync(materials);
            return true;
        }

        public async Task<bool> UpdateLearningMaterialAsync(Guid materialId, LearningMaterialUpdateDto newMaterial)
        {
            if (materialId == null)
                throw new ArgumentNullException(nameof(materialId), "Id bị trống.");

            if (newMaterial == null)
                throw new ArgumentNullException(nameof(newMaterial), "Dữ liệu mới bị trống.");

            var learningMaterial = await _materialRepository.GetLearningMaterialByIdAsync(materialId);

            if (learningMaterial == null)
                throw new KeyNotFoundException($"Không tìm thấy học liệu với ID: {materialId}");

            // Use AutoMapper to update existing entity
            _mapper.Map(newMaterial, learningMaterial);

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                await _materialRepository.UpdateLearningMaterialAsync(learningMaterial);
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }
        public async Task<bool> DeleteLearningMaterialsBySyllabusAsync(Guid syllabusId)
        {
            var materials = await _materialRepository.GetMaterialsBySyllabusIdAsync(syllabusId);

            if (!materials.Any()) return true;

            foreach (var material in materials)
            {
                material.Status = LearningMaterialStatus.Inactive;
                material.UpdatedAt = DateTime.UtcNow;
            }

            await _materialRepository.UpdateLearningMaterialsAsync(materials);

            return true;
        }
        public async Task<List<string>> GetPublishersAsync()
        {
            return await _materialRepository.GetPublishersAsync();
        }
    }
}
