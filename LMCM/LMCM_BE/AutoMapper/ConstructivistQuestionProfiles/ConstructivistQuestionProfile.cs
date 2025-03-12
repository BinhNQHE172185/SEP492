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
            CreateMap<ConstructivistQuestionInsertDto, ConstructivistQuestion>()
                .ForMember(dest => dest.QuestionId, opt => opt.Ignore()) // Ignored since we generate a new ID
                .ForMember(dest => dest.Status, opt => opt.Ignore()) // Set manually
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Set manually
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
            CreateMap<ConstructivistQuestion, ConstructivistQuestionListDto>();
        }
    }
}
