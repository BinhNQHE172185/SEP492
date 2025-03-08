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
        }
    }
}
