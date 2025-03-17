using AutoMapper;
using LMCM_BE.DTOs.PloDtos;
using LMCM_BE.DTOs.PloSubjectDtos;
using LMCM_BE.Models;
using System.Linq;

namespace LMCM_BE.AutoMapper.PloProfiles
{
    public class PloProfile : Profile
    {
        public PloProfile()
        {
            CreateMap<Plo, PloDetailDto>()
            .ForMember(dest => dest.Subjects, opt => opt.MapFrom(src =>
            src.PloSubjects
            .Select(ps => new TempPloSubject
            {
                SubjectId = ps.Subject.SubjectId,
                SubjectCode = ps.Subject.SubjectCode,
                SubjectName = ps.Subject.SubjectName
            })
                .OrderBy(s => s.SubjectCode)
                .ToList()
            ));

            CreateMap<List<Plo>, List<PloDetailDto>>()
            .ConvertUsing(src => src
            .Select(plo => new PloDetailDto
            {
                PloId = plo.PloId,
                PloName = plo.PloName,
                PloDescription = plo.PloDescription,
                Subjects = plo.PloSubjects
            .Select(ps => new TempPloSubject
            {
                SubjectId = ps.Subject.SubjectId,
                SubjectCode = ps.Subject.SubjectCode,
                SubjectName = ps.Subject.SubjectName
            })
            .OrderBy(s => s.SubjectCode)
            .ToList()
            })
            .OrderBy(p => p.PloName)
            .ToList()
            );
        }
    }
}
