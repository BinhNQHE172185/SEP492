using LMCM_BE.DTOs.DashboardDtos;
using LMCM_BE.Services.CurriculumService;
using LMCM_BE.Services.LearningMaterialChangesHistoryService;
using LMCM_BE.Services.SubjectService;
using LMCM_BE.Services.SyllabusService;
using LMCM_BE.Services.UserService;

namespace LMCM_BE.Services.DashboardService
{
    public class DashboardService : IDashboardService
    {
        private readonly ILearningMaterialChangesHistorySerivce _changesService;
        private readonly IUserService _userService;
        private readonly ISubjectService _subjectService;
        private readonly ICurriculumService _curriculumService;
        private readonly ISyllabusService _syllabusService;

        public DashboardService(
            ILearningMaterialChangesHistorySerivce changesService,
            IUserService userService,
            ISubjectService subjectService,
            ICurriculumService curriculumService,
            ISyllabusService syllabusService)
        {
            _changesService = changesService;
            _userService = userService;
            _subjectService = subjectService;
            _curriculumService = curriculumService;
            _syllabusService = syllabusService;
        }

        public async Task<ChartDataDto> GetPieChartDataAsync()
        {
            var histories = await _changesService.GetAllWithCompletionDateAsync();

            var now = DateTime.UtcNow;

            var result = histories
                .Select(h => new
                {
                    Range = (now.Year - h.CompletionDate.Value.Year) switch
                    {
                        >= 6 => "Trên 5 năm",
                        >= 3 => "Từ 3 đến 5 năm",
                        _ => "Dưới 3 năm"
                    }
                })
                .GroupBy(x => x.Range)
                .Select(g => new { Label = g.Key, Count = g.Count() })
                .ToList();

            var labels = new List<string> { "Trên 5 năm", "Từ 3 đến 5 năm", "Dưới 3 năm" };
            var data = labels.Select(label => result.FirstOrDefault(r => r.Label == label)?.Count ?? 0).ToList();

            return new ChartDataDto
            {
                Labels = labels,
                Data = data
            };
        }
        public async Task<ChartDataDto> GetColumnChartDataAsync()
        {
            var histories = await _changesService.GetAllWithCompletionDateAsync();

            var groupedByYear = histories
                .Where(h => h.CompletionDate.HasValue)
                .GroupBy(h => h.CompletionDate.Value.Year)
                .OrderBy(g => g.Key)
                .Select(g => new
                {
                    Year = g.Key.ToString(),
                    Count = g.Count()
                })
                .ToList();

            var dto = new ChartDataDto
            {
                Labels = groupedByYear.Select(x => x.Year).ToList(),
                Data = groupedByYear.Select(x => x.Count).ToList()
            };

            return dto;
        }
        public async Task<ItemsDto> GetItemsAsync()
        {
            var data = new ItemsDto
            {
                subjectCount = await _subjectService.getSubjectCountAsync(),
                syllabusCount = await _syllabusService.getSyllabusCountAsync(),
                userCount = await _userService.GetActiveUserCountAsync(),
                curriculumnCount = await _curriculumService.getCurriculumnCountAsync()
            };

            return data;
        }
    }
}
