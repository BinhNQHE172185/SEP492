using AutoMapper;
using LMCM_BE.DTOs.AcceptanceRecordDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Repositories.AcceptanceRecordRepository;

namespace LMCM_BE.Services.AcceptanceRecordService
{
    public class AcceptanceRecordService : IAcceptanceRecordService
    {
        private readonly IAcceptanceRecordRepository _acceptanceRecordRepository;
        private readonly IMapper _mapper;

        public AcceptanceRecordService(IAcceptanceRecordRepository acceptanceRecordRepository, IMapper mapper)
        {
            _acceptanceRecordRepository = acceptanceRecordRepository;
            _mapper = mapper;
        }

        public async Task<PagedResult<AcceptanceRecordListDto>> GetAcceptanceRecordsAsync(string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            return await _acceptanceRecordRepository.GetAcceptanceRecordsAsync(searchKey, pageIndex, pageSize);
        }

        public async Task<AcceptanceRecordDetailDto> CreateAcceptanceRecordAsync(AcceptanceRecordCreateDto dto)
        {
            return await _acceptanceRecordRepository.CreateAcceptanceRecordAsync(dto);
        }

        public async Task<Guid?> UpdateAcceptanceRecordAsync(Guid acceptanceId, AcceptanceRecordUpdateDto dto)
        {
            return await _acceptanceRecordRepository.UpdateAcceptanceRecordAsync(acceptanceId, dto);
        }

        public async Task<bool> SoftDeleteAcceptanceRecordAsync(Guid acceptanceId)
        {
            return await _acceptanceRecordRepository.SoftDeleteAcceptanceRecordAsync(acceptanceId);
        }

        public async Task<AcceptanceRecordDetailDto> GetAcceptanceRecordDetailAsync(Guid acceptanceId)
        {
            return await _acceptanceRecordRepository.GetAcceptanceRecordDetailAsync(acceptanceId);
        }
    }
}
