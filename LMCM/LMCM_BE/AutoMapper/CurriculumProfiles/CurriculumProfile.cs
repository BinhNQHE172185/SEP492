using AutoMapper;
using LMCM_BE.DTOs.CurriculumDtos;
using LMCM_BE.Models;
using System.Linq;

namespace LMCM_BE.AutoMapper.CurriculumProfiles
{
    public class CurriculumProfile : Profile
    {
        public CurriculumProfile()
        {
            CreateMap<Curriculum, CurriculumDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.CurriculumName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.CurriculumDescription))
                .ForMember(dest => dest.TotalCredit, opt => opt.MapFrom(src => src.CurriculumsSubjects.Sum(cs => cs.Credit ?? 0)))
                .ReverseMap();
        }
    }
}
