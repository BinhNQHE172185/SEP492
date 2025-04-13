using AutoMapper;
using LMCM_BE.DTOs.GradingStructureDtos;
using LMCM_BE.Models;

namespace LMCM_BE.AutoMapper.GradingStructureProfiles
{
    public class GradingStructureProfile:Profile
    {
        public GradingStructureProfile()
        {
            CreateMap<GradingStructure, GradingStructureDto>();
            CreateMap<GradingStructureDto, GradingStructure>()
                .ForMember(dest => dest.StructureId, opt => opt.Ignore()) // Ignored since we generate a new ID
                .ForMember(dest => dest.Status, opt => opt.Ignore()) // Set manually
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Set manually
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
        }
    }
}
