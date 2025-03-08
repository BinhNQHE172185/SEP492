using AutoMapper;
using LMCM_BE.DTOs.GradingStructureDtos;
using LMCM_BE.Models;

namespace LMCM_BE.AutoMapper.GradingStructureProfiles
{
    public class GradingStructureProfile:Profile
    {
        public GradingStructureProfile()
        {
            CreateMap<GradingStructure, GradingStructureInsertDto>();
        }
    }
}
