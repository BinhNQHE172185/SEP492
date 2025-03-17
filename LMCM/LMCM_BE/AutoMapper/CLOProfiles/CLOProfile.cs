using AutoMapper;
using LMCM_BE.DTOs.CLODtos;
using LMCM_BE.DTOs.SubjectDtos;
using LMCM_BE.Models;

namespace LMCM_BE.AutoMapper.CLOProfiles
{
    public class CLOProfile : Profile
    {
        public CLOProfile()
        {
            CreateMap<Clo, CLOInsertDto>();
            CreateMap<CLOInsertDto, Clo>()
                .ForMember(dest => dest.CloId, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore()) 
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) 
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
            CreateMap<Clo, CLOListDto>();
        }
    }
}
