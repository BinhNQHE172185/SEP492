using AutoMapper;
using LMCM_BE.DTOs.ContractorDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Models;
using LMCM_BE.Repositories.ContractorRepository;
using LMCM_BE.Repositories.ContractRepository;
using LMCM_BE.Shared.Constant;
using LMCM_BE.UnitOfWork;

namespace LMCM_BE.Services.ContractorService
{
    public class ContractorService : IContractorService
    {
        private readonly IContractorRepository _contractorRepository;
        private readonly IContractRepository _contractRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ContractorService(
            IContractorRepository contractorRepository,
            IContractRepository contractRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork
            )
        {
            _contractorRepository = contractorRepository;
            _contractRepository = contractRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResult<ContractorListDto>> GetContractorsAsync(string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            var (items, totalCount) = await _contractorRepository.GetContractorsAsync(searchKey, pageIndex, pageSize);
            var data = _mapper.Map<List<ContractorListDto>>(items);

            return new PagedResult<ContractorListDto>
            {
                Items = data,
                TotalCount = totalCount,
                CurrentPage = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<List<ContractorListDto>> GetContractorsListAsync(string? searchKey)
        {
            var items = await _contractorRepository.GetContractorsListAsync(searchKey);

            return _mapper.Map<List<ContractorListDto>>(items);
        }

        public async Task<bool> SoftDeleteContractorAsync(Guid contractorId)
        {
            if (contractorId == Guid.Empty)
                throw new ArgumentException("ID nhà thầu không được để trống.", nameof(contractorId));

            var contractor = await _contractorRepository.GetActiveContractorByIdAsync(contractorId);

            if (contractor == null)
                throw new KeyNotFoundException("Không tìm thấy nhà thầu hoặc đã bị xóa.");

            if (await _contractRepository.HasActiveContractsAsync(contractorId))
                throw new InvalidOperationException("Không thể xóa nhà thầu khi vẫn có hợp đồng đang hoạt động.");

            contractor.Status = GenericStatus.Inactive;
            contractor.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                await _contractorRepository.UpdateContractorAsync(contractor);
                await _unitOfWork.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }

        public async Task<ContractorDetailDto?> GetContractorDetailAsync(Guid contractorId)
        {
            var contractor = await _contractorRepository.GetContractorDetailByIdAsync(contractorId);

            if (contractor == null)
                throw new KeyNotFoundException("Không tìm thấy nhà thầu.");

            return _mapper.Map<ContractorDetailDto>(contractor);
        }

        public async Task<bool> CreateContractorAsync(ContractorCreateDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto), "Dữ liệu nhà thầu là bắt buộc.");
            }

            var duplicatedContractorId = await _contractorRepository.GetDuplicatedContractorIdAsync(
                   dto.TaxCode,
                   dto.EmployeeCode,
                   dto.IdCardNumber,
                   dto.Email
               );

            if (duplicatedContractorId != null)
            {
                throw new InvalidOperationException("Nhà thầu đã tồn tại với mã số thuế, mã nhân viên, số CMND hoặc email đã cung cấp.");
            }

            var contractor = _mapper.Map<Contractor>(dto);
            contractor.ContractorId = Guid.NewGuid();
            contractor.Status = GenericStatus.Active;
            contractor.CreatedAt = DateTime.UtcNow;
            contractor.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                await _contractorRepository.CreateContractorAsync(contractor);
                await _unitOfWork.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }

        public async Task<Guid?> UpdateContractorAsync(Guid contractorId, ContractorUpdateDto dto)
        {
            if (contractorId == Guid.Empty)
                throw new ArgumentException("ID nhà thầu không được để trống.", nameof(contractorId));

            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto), "Dữ liệu nhà thầu là bắt buộc.");
            }

            var duplicatedContractorId = await _contractorRepository.GetDuplicatedContractorIdAsync(
                   dto.TaxCode,
                   dto.EmployeeCode,
                   dto.IdCardNumber,
                   dto.Email
               );

            if (duplicatedContractorId != null && duplicatedContractorId != contractorId)
            {
                throw new InvalidOperationException("Nhà thầu đã tồn tại với mã số thuế, mã nhân viên, số CMND hoặc email đã cung cấp.");
            }

            var contractor = await _contractorRepository.GetActiveContractorByIdAsync(contractorId);

            if (contractor == null)
                throw new KeyNotFoundException("Không tìm thấy nhà thầu hoặc đã bị xóa.");

            _mapper.Map(dto, contractor);
            contractor.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                await _contractorRepository.UpdateContractorAsync(contractor);
                await _unitOfWork.CommitAsync();

                return contractorId;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }
        public async Task<Guid?> CheckContractor(string taxcode, string? email, string? phoneNumber)
        {
            return await _contractorRepository.CheckContractor(taxcode, email, phoneNumber);
        }
    }
}
