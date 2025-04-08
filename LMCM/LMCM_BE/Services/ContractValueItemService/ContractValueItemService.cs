using AutoMapper;
using LMCM_BE.DTOs.ContractValueItemDtos;
using LMCM_BE.Models;
using LMCM_BE.Repositories.ContractValueItemRepository;
using LMCM_BE.UnitOfWork;

namespace LMCM_BE.Services.ContractValueItemService
{
    public class ContractValueItemService : IContractValueItemService
    {
        private readonly IContractValueItemRepository _contractValueItemRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ContractValueItemService(
            IContractValueItemRepository contractValueItemRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper
            )
        {
            _contractValueItemRepository = contractValueItemRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<ContractValueItem>> GetListAsync()
        {
            return await _contractValueItemRepository.GetListAsync();
        }

        public async Task<bool> UpdateAsync(List<ContractValueItemDto> newItems)
        {
            if (newItems == null)
            {
                throw new ArgumentException("Danh sách không được để trống.");
            }
            var existingItems = await _contractValueItemRepository.GetListAsync();

            var toDelete = existingItems.Where(e => !newItems.Any(n => n.ValueId == e.ValueId)).ToList();
            var toUpdate = new List<ContractValueItem>();
            var toAdd = new List<ContractValueItem>();
            foreach (var newItem in newItems)
            {
                var existingItem = existingItems.FirstOrDefault(e => e.ValueId == newItem.ValueId);
                if (existingItem != null)
                {
                    _mapper.Map(newItem, existingItem);
                    toUpdate.Add(existingItem);
                }
                else
                {
                    toAdd.Add(_mapper.Map<ContractValueItem>(newItem));
                }
            }
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                if (toDelete.Any()) await _contractValueItemRepository.DeleteRangeAsync(toDelete);
                if (toUpdate.Any()) await _contractValueItemRepository.UpdateRangeAsync(toUpdate);
                if (toAdd.Any()) await _contractValueItemRepository.AddRangeAsync(toAdd);
                await _unitOfWork.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }
    }
}
