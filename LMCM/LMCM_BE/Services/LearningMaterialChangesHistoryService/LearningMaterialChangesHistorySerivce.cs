using AutoMapper;
using LMCM_BE.DTOs.LearningMaterialChangesHistoryDtos;
using LMCM_BE.DTOs.LearningMaterialDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.DTOs.UserDtos;
using LMCM_BE.Models;
using LMCM_BE.Repositories.ContractRepository;
using LMCM_BE.Repositories.LearningMaterialChangesHistoryRepository;
using LMCM_BE.Repositories.SyllabusRepository;
using LMCM_BE.Repositories.UserRepositoriy;
using LMCM_BE.Services.UserService;
using LMCM_BE.Shared.Constant;
using LMCM_BE.UnitOfWork;

namespace LMCM_BE.Services.LearningMaterialChangesHistoryService
{
    public class LearningMaterialChangesHistorySerivce : ILearningMaterialChangesHistorySerivce
    {
        private readonly ILearningMaterialChangesHistoryRepository _changesRepository;
        private readonly ISyllabusRepository _syllabusRepository;
        private readonly IContractRepository _contractRepository;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;

        public LearningMaterialChangesHistorySerivce(
            ILearningMaterialChangesHistoryRepository changesRepository,
            ISyllabusRepository syllabusRepository,
            IContractRepository contractRepository,
            IMapper mapper,
            IUserService userService,
            IUnitOfWork unitOfWork
            )
        {
            _changesRepository = changesRepository;
            _syllabusRepository = syllabusRepository;
            _contractRepository = contractRepository;
            _mapper = mapper;
            _userService = userService;
            _unitOfWork = unitOfWork;
        }
        public async Task<PagedResult<ChangesHistoryListDto>> GetChangesHistoriesAsync(string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            var (items, totalCount) = await _changesRepository.GetChangesHistoriesAsync(searchKey, pageIndex, pageSize);
            var data = _mapper.Map<List<ChangesHistoryListDto>>(items);

            return new PagedResult<ChangesHistoryListDto>
            {
                Items = data,
                TotalCount = totalCount,
                CurrentPage = pageIndex,
                PageSize = pageSize
            };
        }
        public async Task<bool> CreateLearningMaterialChangesHistoryAsync(CreateLearningMaterialChangesHistoryDto historyDto)
        {
            if (historyDto == null)
            {
                throw new ArgumentNullException(nameof(historyDto));
            }
            UserProfileResponseDto user = await _userService.GetProfileFromCookie();
            if (user == null || string.IsNullOrEmpty(user.Email))
                throw new Exception("User not found");

            if (historyDto.ContractId != null)
            {
                if (await _contractRepository.GetActiveContractByIdAsync(historyDto.ContractId.Value) == null)
                {
                    throw new KeyNotFoundException("Hợp đồng được chọn không tồn tại.");
                }
            }
            if (await _syllabusRepository.GetActiveSyllabusByIdAsync(historyDto.SyllabusId) == null)
            {
                throw new KeyNotFoundException("Đề cương được chọn không tồn tại.");
            }

            var history = _mapper.Map<LearningMaterialChangesHistory>(historyDto);
            history.UserId = user.Id;
            history.HistoryId = Guid.NewGuid();
            history.Status = GenericStatus.Active;

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                await _changesRepository.CreateLearningMaterialChangesHistoryAsync(history);
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }
        public async Task<PagedResult<ChangesHistoryOfSubjectDto>> GetLearningMaterialChangesHistoriesOfSubjectAsync(
    Guid? subjectId, string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            if (subjectId == Guid.Empty)
                throw new ArgumentException("ID môn học không được để trống.", nameof(subjectId));

            var siblingSyllabus = await _syllabusRepository.GetSyllabusesBySubjectIdAsync(subjectId.Value);

            var (items, totalCount) = await _changesRepository.GetLearningMaterialChangesHistoriesOfSubjectAsync(siblingSyllabus, searchKey, pageIndex, pageSize);

            var data = _mapper.Map<List<ChangesHistoryOfSubjectDto>>(items);

            return new PagedResult<ChangesHistoryOfSubjectDto>
            {
                Items = data,
                TotalCount = totalCount,
                CurrentPage = pageIndex,
                PageSize = pageSize
            };
        }
        public async Task<bool> UpdateLearningMaterialChangesHistoryAsync(Guid historyId, UpdateLearningMaterialChangesHistoryDto dto)
        {
            if (historyId == Guid.Empty)
                throw new ArgumentException("ID lịch sử thay đổi không được để trống.", nameof(historyId));

            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Dữ liệu cập nhật không được để trống.");

            var existingHistory = await _changesRepository.GetActiveHistoryByIdAsync(historyId);

            if (existingHistory == null)
                throw new Exception("Không tìm thấy lịch sử thay đổi.");

            UserProfileResponseDto user = await _userService.GetProfileFromCookie();
            if (user == null || string.IsNullOrEmpty(user.Email))
                throw new Exception("Không tìm thấy người dùng");

            if (user.Id != existingHistory.UserId && !user.Roles.Contains("Head of Department"))
                throw new UnauthorizedAccessException("Người dùng không có quyền cập nhật lịch sử này.");

            if (dto.ContractId != null)
            {
                if (await _contractRepository.GetActiveContractByIdAsync(dto.ContractId.Value) == null)
                {
                    throw new KeyNotFoundException("Hợp đồng được chọn không tồn tại.");
                }
            }
            if (await _syllabusRepository.GetActiveSyllabusByIdAsync(dto.SyllabusId) == null)
            {
                throw new KeyNotFoundException("Đề cương được chọn không tồn tại.");
            }

            _mapper.Map(dto, existingHistory);

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                await _changesRepository.UpdateLearningMaterialChangesHistoryAsync(existingHistory);
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }
        public async Task<bool> SoftDeleteLearningMaterialChangesHistoryAsync(Guid historyId)
        {
            var historyRecord = await _changesRepository.GetActiveHistoryByIdAsync(historyId);

            if (historyRecord == null)
                throw new KeyNotFoundException("Không tìm thấy lịch sử thay đổi hoặc đã bị xóa.");

            UserProfileResponseDto user = await _userService.GetProfileFromCookie();
            if (user == null || string.IsNullOrEmpty(user.Email))
                throw new Exception("Không tìm thấy người dùng");

            if (user.Id != historyRecord.UserId && !user.Roles.Contains("Head of Department"))
                throw new UnauthorizedAccessException("Người dùng không có quyền xóa lịch sử này.");

            historyRecord.Status = GenericStatus.Inactive;

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                await _changesRepository.UpdateLearningMaterialChangesHistoryAsync(historyRecord);
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }
        public async Task<ChangesHistoryDetailDto> getHistoryOfChangeDetail(Guid id)
        {
            var history = await _changesRepository.getHistoryOfChangeDetail(id);
            if (history == null)
            {
                throw new KeyNotFoundException("Không tìm thấy lịch sử thay đổi.");
            }
            return _mapper.Map<ChangesHistoryDetailDto>(history);
        }
        public async Task<List<LearningMaterialChangesHistory>> GetAllWithCompletionDateAsync()
        {
            return await _changesRepository.GetAllWithCompletionDateAsync();
        }
    }
}
