using AutoMapper;
using LMCM_BE.DTOs.ContractValueItemDtos;
using LMCM_BE.Models;

namespace LMCM_BE.AutoMapper.ContractValueItemProfiles
{
    public class ContractValueItemProfile : Profile
    {
        public ContractValueItemProfile()
        {
            CreateMap<ContractValueItemDto, ContractValueItem>();

            CreateMap<ContractValueItem, ContractValueItemDto>();
        }
    }
}
