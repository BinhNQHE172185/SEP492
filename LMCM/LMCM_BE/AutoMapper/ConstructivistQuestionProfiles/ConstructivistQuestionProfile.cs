using AutoMapper;
using LMCM_BE.DTOs.ConstructivistQuestionDtos;
using LMCM_BE.Models;

namespace LMCM_BE.AutoMapper.ConstructivistQuestionProfiles
{
    public class ConstructivistQuestionProfile : Profile
    {
        public ConstructivistQuestionProfile()
        {
            CreateMap<ConstructivistQuestion, ConstructivistQuestionInsertDto>();
        }
    }
}
